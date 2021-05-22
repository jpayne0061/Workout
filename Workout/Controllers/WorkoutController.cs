using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Workout.Data;
using Workout.Models;
using Workout.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Workout.Controllers
{
    public class WorkoutController : Controller
    {
        ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WorkoutController(ApplicationDbContext workoutContext, UserManager<IdentityUser> userManager)
        {
            _context = workoutContext;
            _userManager = userManager;
        }

        // GET: Workout
        public async Task<ActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View("NotAuthenticated");
            }

            IdentityUser user = await _userManager.GetUserAsync(User);

            var workouts = _context.Workout.Where(w => w.AspNetUserId == user.Id).ToList();

            var exercises = _context.Exercise.ToDictionary(x => x.ExerciseId, x => x);

            foreach (var w in workouts)
            {
                w.WorkoutToExercise = _context.WorkoutToExercise.Where(we => we.WorkoutId == w.WorkoutId && we.Active).ToList();

                foreach (var we in w.WorkoutToExercise)
                {
                    we.Exercise = exercises[we.ExerciseId];
                }

                w.WorkoutSet = _context.WorkoutSet.Where(ws => ws.WorkoutId == w.WorkoutId && ws.Active).OrderBy(ws => ws.SetOrder).ToList();
            }

            List<WorkoutVM> workoutViewModels = new List<WorkoutVM>();

            foreach (var w in workouts)
            {
                WorkoutVM workoutVM = new WorkoutVM(w);

                Dictionary<string, List<string>> exerciseToSets = new Dictionary<string, List<string>>();

                List<WorkoutToExercise> wtes = w.WorkoutToExercise.OrderBy(x => x.Order).ToList();

                foreach (var wte in wtes)
                {
                    foreach (var ws in wte.Workout.WorkoutSet.Where(ws => ws.ExerciseId == wte.ExerciseId && ws.Active).OrderBy(x => x.SetOrder))
                    {
                        if (exerciseToSets.ContainsKey(ws.Exercise.ExerciseName))
                        {
                            exerciseToSets[ws.Exercise.ExerciseName].Add(ws.Reps.ToString());
                        }
                        else
                        {
                            exerciseToSets[ws.Exercise.ExerciseName] = new List<string> { ws.Reps.ToString() };
                        }
                    }

                }

                workoutVM.ExerciseDisplay = exerciseToSets;

                workoutViewModels.Add(workoutVM);
            }

            return View(workoutViewModels);
        }

        [Route("{workoutId:int}")]
        public ActionResult ViewHistory([FromRoute] int workoutId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View("NotAuthenticated");
            }

            var workout = _context.Workout.Where(w => w.WorkoutId == workoutId).First();

            var workoutToExercise = _context.WorkoutToExercise.Where(wte => wte.WorkoutId == workoutId).ToList();

            var exercises = _context.Exercise.Where(e => workoutToExercise.Select(x => x.ExerciseId).Contains(e.ExerciseId)).ToList();

            var workoutSets = _context.WorkoutSet.Where(ws => ws.WorkoutId == workoutId).ToList();

            var workoutSetResults = _context.WorkoutSetResult.Where(wsr => workoutSets.Select(x => x.WorkoutSetId).Contains(wsr.WorkoutSetId)).ToList();

            List<WorkoutSessionVM> workoutSessions = new List<WorkoutSessionVM>();

            Dictionary<int, List<WorkoutSetResult>> workoutResultsGroupedByWorkoutSessionId = workoutSetResults
                                                                                .GroupBy(x => x.WorkoutSessionId).ToDictionary(x => x.Key, x => x.ToList());

            foreach (var wsr in workoutSetResults)
            {
                wsr.WorkoutSet = workoutSets.Where(ws => ws.WorkoutSetId == wsr.WorkoutSetId).First();

                wsr.WorkoutSet.Exercise = exercises.Where(e => e.ExerciseId == wsr.WorkoutSet.ExerciseId).First();
            }



            foreach (var kvp in workoutResultsGroupedByWorkoutSessionId)
            {
                List<ExerciseResultsHistory> exerciseResultsHistories = new List<ExerciseResultsHistory>();

                List<WorkoutSetResult> setResults = kvp.Value;

                DateTime? date = setResults.First()?.DateCreated;

                var exerciseIdToResults = setResults.GroupBy(x => x.WorkoutSet.ExerciseId).ToDictionary(x => x.Key, x => x.OrderBy(z => z.DateCreated).ToList());

                foreach (var etr in exerciseIdToResults)
                {
                    ExerciseResultsHistory exerciseResultsHistory = new ExerciseResultsHistory();

                    exerciseResultsHistory.ExerciseName = exercises.Where(e => e.ExerciseId == etr.Key).First().ExerciseName;

                    var i = 1;

                    foreach (WorkoutSetResult result in etr.Value)
                    {
                        var resultLine = $"Set {i}: Weight = {result.Weight} Reps = {result.RepsCompleted}";

                        if(result.Skipped)
                        {
                            resultLine = $"Set {i}: skipped";
                        }

                        exerciseResultsHistory.Results.Add(resultLine);

                        i++;
                    }

                    exerciseResultsHistories.Add(exerciseResultsHistory);
                }

                WorkoutSessionVM workoutSessionVM = new WorkoutSessionVM();

                workoutSessionVM.WorkoutSessionDate = workoutSetResults.Where(x => x.WorkoutSessionId == kvp.Key).First().DateCreated;

                workoutSessionVM.ExerciseHistories = exerciseResultsHistories;

                workoutSessions.Add(workoutSessionVM);
            }

            workoutSessions = workoutSessions.OrderByDescending(ws => ws.WorkoutSessionDate).ToList();

            return View("WorkoutHistory", workoutSessions);
        }

        public ActionResult Exercises()
        {
            return View();
        }

        // GET: Workout/Details/5
        public ActionResult Details(int id)
        {
            //if (id == null)
            //{
            //    return new HttpStatus(HttpStatusCode.BadRequest);
            //}

            Workout.Models.Workout workout = _context.Workout.Find(id);

            //if (student == null)
            //{
            //    return HttpNotFound();
            //}


            return View(workout);
        }

        // GET: Workout/Create
        public ActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View("NotAuthenticated");
            }

            var vm = new WorkoutVM();

            vm.Workout = new Workout.Models.Workout();
            vm.AllExercises = _context.Exercise.ToList().Select(x => new ExerciseVM { ExerciseID = x.ExerciseId, ExerciseName = x.ExerciseName }).ToList();

            return View(vm);
        }


        public ActionResult CreateExercise()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateExercise(Exercise exercise)
        {

            exercise.DateCreated = DateTime.Now;

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Exercise.Add(exercise);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View();
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] WorkoutVM workoutVM)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View("NotAuthenticated");
            }

            var workout = new Workout.Models.Workout();

            workout.DateCreated = DateTime.Now;
            workout.CreatedBy = 1;
            workout.WorkoutName = workoutVM.Workout.WorkoutName;
            workout.WorkoutToExercise = workoutVM.AllExercises.Where(e => e.Selected).Select(x => new WorkoutToExercise { Workout = workout, ExerciseId = x.ExerciseID, Order = x.Order, Active = true }).ToList();
            workoutVM.AllExercises = workoutVM.AllExercises.Where(e => e.Selected).ToList();
            IdentityUser user = await _userManager.GetUserAsync(User);
            workout.AspNetUserId = user.Id;

            workout.WorkoutSet = GetWorkOutSets(workoutVM, workout);

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Workout.Add(workout);
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(workout);
        }

        public List<WorkoutSet> GetWorkOutSets(WorkoutVM workoutVM, Workout.Models.Workout workout)
        {
            List<WorkoutSet> workoutSets = new List<WorkoutSet>();

            foreach (var exercise in workoutVM.AllExercises)
            {
                List<int> sets = exercise.SetsAndReps
                    .Split(new char[] { ' ', ',' })
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => int.Parse(x))
                    .ToList();

                for (int i = 0; i < sets.Count; i++)
                {
                    WorkoutSet workoutSet = new WorkoutSet();

                    workoutSet.Reps = sets[i];
                    workoutSet.SetOrder = i + 1;
                    workoutSet.ExerciseId = exercise.ExerciseID;
                    workoutSet.DateCreated = DateTime.Now;
                    workoutSet.Workout = workout;
                    workoutSet.ExerciseId = exercise.ExerciseID;
                    workoutSet.Active = true;

                    workoutSets.Add(workoutSet);
                }
            }

            return workoutSets;
        }

        private async Task<InProgressSession> GetInProgressSession()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);

            //get workout

            //determine if workout is complete

            //could be a problem, if they are on their last set and move

            //need to determine if last set if we are going to redirect to start set

            return new InProgressSession();
        }

        [Route("{workoutId:int}/{workoutSetId:int}/{lastSet:bool}/{currentExerciseId:int}/{workoutSessionId:int}")]
        public ActionResult StartSet([FromRoute] int workoutId, int workoutSetId, bool lastSet, int currentExerciseId, int workoutSessionId)
        {
            //check if session is active
            //if so, get workoutid, workoutsetid, lastset, current 

            int exerciseId = 0;

            Exercise exercise = null;

            List<WorkoutToExercise> wtes = _context.WorkoutToExercise
                .Where(we => we.WorkoutId == workoutId && we.Active)
                .OrderBy(wte => wte.Order).ToList();

            List<Exercise> exercises = _context.Exercise
                .Where(e => wtes.Select(wte => wte.ExerciseId)
                .Contains(e.ExerciseId)).ToList();

            //get last session
            WorkoutSession lastWorkoutSession = null;

            WorkoutSet currentWorkoutSet = null;

            WorkoutSet nextWorkoutSet = null;

            List<WorkoutSet> workoutSets = null;

            bool needToGetCurrentSessionInfo = false;

            if(workoutSessionId == 0)
            {
                lastWorkoutSession = _context.WorkoutSession.Where(ws => (DateTime.Now - ws.DateCreated).TotalHours < 1).FirstOrDefault();

                if(lastWorkoutSession != null)
                {
                    needToGetCurrentSessionInfo = true;
                }

            }

            List<WorkoutSetResult> workoutSetResults = null;

            if(lastWorkoutSession != null)
            {
                workoutSetResults = _context.WorkoutSetResult.Where(wsr => wsr.WorkoutSessionId == lastWorkoutSession.WorkoutSessionId).ToList();
            }
            

            //need to check if session started with different workout



            //if(workoutSetResults.Where(wsr => wsr.))
            //{

            //}

            if (needToGetCurrentSessionInfo)
            {
                

                wtes = _context.WorkoutToExercise
                        .Where(we => we.WorkoutId == workoutId && we.Active)
                        .OrderBy(wte => wte.Order).ToList();

                var lastCompletedWorkoutSetResult = workoutSetResults.OrderByDescending(wsr => wsr.WorkoutSetResultId).First();

                
                workoutSetId = lastCompletedWorkoutSetResult.WorkoutSetId;

                var currentWorkoutSetHere = _context.WorkoutSet
                                .Where(ws => ws.WorkoutSetId == workoutSetId)
                                .Single();

                //is lastCompletedWorkoutSetResult the last set?
                currentExerciseId = currentWorkoutSetHere.ExerciseId;

                var workoutSetsHere = _context.WorkoutSet.Where(ws => ws.WorkoutId == workoutId && ws.ExerciseId == currentExerciseId).ToList();


                if(workoutSetsHere.Count == 0) //user has started session, but switched workouts.what if workout edited in middle of session?
                {
                    workoutSetId = 0;
                    lastSet = false;
                    currentExerciseId = 0;
                }
                else if(workoutSetsHere.Count == 1)
                {
                    lastSet = true;
                }
                else
                {
                    lastSet = workoutSetsHere.OrderByDescending(wsh => wsh.SetOrder).First().WorkoutSetId == workoutSetId;
                }

            }


            if(workoutSessionId == 0 && lastWorkoutSession != null)
            {
                workoutSessionId = lastWorkoutSession.WorkoutSessionId;
            }

            if (workoutSessionId == 0)
            {
                WorkoutSession workoutSession = new WorkoutSession { DateCreated = DateTime.Now };

                _context.WorkoutSession.Add(workoutSession);

                _context.SaveChanges();

                workoutSessionId = workoutSession.WorkoutSessionId;
            }



            bool workoutComplete = false;

            if (workoutSetId == 0) //firstSet
            {
                exerciseId = wtes.First().ExerciseId;
            }
            else if (lastSet) //last set -- need next exercise
            {
                int currentOrder = wtes.Where(wte => wte.ExerciseId == currentExerciseId).First().Order;

                WorkoutToExercise workoutToExercise = wtes.FirstOrDefault(wte => wte.Order > currentOrder);

                if (workoutToExercise == null)
                {
                    workoutComplete = true;
                }
                else
                {
                    exerciseId = workoutToExercise.ExerciseId;//is null posible here?
                }
            }
            else
            {
                exerciseId = currentExerciseId;
            }

            if (workoutComplete)
            {
                return View("WorkoutComplete");
            }

            exercise = exercises.Where(e => e.ExerciseId == exerciseId).Single();

            workoutSets = _context.WorkoutSet
                .Where(ws => ws.WorkoutId == workoutId && ws.ExerciseId == exerciseId && ws.Active)
                .OrderBy(x => x.SetOrder).ToList();

            

            bool nextSetIsLast = false;

            

            SetViewModel setViewModel = null;

            PreviousSetResult previousResult = null;

            if (workoutSetId == 0 || lastSet)
            {
                workoutSetId = workoutSets.Where(ws => ws.ExerciseId == exercise.ExerciseId).OrderBy(ws => ws.SetOrder).First().WorkoutSetId;

                nextWorkoutSet = workoutSets.Where(ws => ws.WorkoutSetId == workoutSetId).First();

                nextSetIsLast = nextWorkoutSet.SetOrder == workoutSets.Select(ws => ws.SetOrder).Max();

                previousResult = GetLastWorkoutSetResult(nextWorkoutSet.WorkoutSetId, workoutSessionId);

                if (previousResult.WorkoutSetResult != null && (DateTime.Now - previousResult.WorkoutSetResult.DateCreated).TotalMinutes < 60)
                {
                    workoutSessionId = previousResult.WorkoutSetResult.WorkoutSessionId;
                }

                setViewModel = new SetViewModel(previousResult);

                setViewModel.Exercise = exercise;
                setViewModel.WorkoutSet = nextWorkoutSet;
                setViewModel.WorkoutId = workoutId;
                setViewModel.IsLastSet = nextSetIsLast;

                ModelState.SetModelValue("workoutSessionId", new ValueProviderResult(new Microsoft.Extensions.Primitives.StringValues(workoutSessionId.ToString()), CultureInfo.InvariantCulture));

                return View("Set", setViewModel);
            }

            currentWorkoutSet = workoutSets.Where(ws => ws.WorkoutSetId == workoutSetId).First();

            nextWorkoutSet = workoutSets.Where(ws => ws.ExerciseId == exerciseId && ws.SetOrder > currentWorkoutSet.SetOrder).First();

            nextSetIsLast = nextWorkoutSet.SetOrder == workoutSets.Select(ws => ws.SetOrder).Max();

            previousResult = GetLastWorkoutSetResult(nextWorkoutSet.WorkoutSetId, workoutSessionId);

            if (previousResult.WorkoutSetResult != null && (DateTime.Now - previousResult.WorkoutSetResult.DateCreated).TotalMinutes < 60)
            {
                workoutSessionId = previousResult.WorkoutSetResult.WorkoutSessionId;
            }

            setViewModel = new SetViewModel(previousResult);

            setViewModel.Exercise = exercise;
            setViewModel.WorkoutSet = nextWorkoutSet;
            setViewModel.WorkoutId = workoutId;
            setViewModel.IsLastSet = nextSetIsLast;

            ModelState.SetModelValue("workoutSessionId", new ValueProviderResult(new Microsoft.Extensions.Primitives.StringValues(workoutSessionId.ToString()), CultureInfo.InvariantCulture));

            return View("Set", setViewModel);
        }


        public PreviousSetResult GetLastWorkoutSetResult(int workoutSetId, int workoutSessionId)
        {
            bool fromSameSession = false;

            var lastWorkoutSetResult = _context.WorkoutSetResult.Where(wsr => wsr.WorkoutSetId == workoutSetId)
                                             .OrderByDescending(x => x.DateCreated).FirstOrDefault();

            if(lastWorkoutSetResult == null)
            {
                lastWorkoutSetResult = _context.WorkoutSetResult.Where(wsr => wsr.WorkoutSessionId == workoutSessionId)
                                             .OrderByDescending(x => x.DateCreated).FirstOrDefault();

                fromSameSession = true;
            }

            return new PreviousSetResult { WorkoutSetResult = lastWorkoutSetResult, FromSameSession = fromSameSession };
        }


        [HttpPost]
        public ActionResult CompleteSet([FromForm] SetViewModel setViewModel)
        {
            WorkoutSetResult workoutSetResult = new WorkoutSetResult();

            workoutSetResult.DateCreated = DateTime.Now;
            workoutSetResult.Difficulty = (int)setViewModel.Difficulty;
            workoutSetResult.Notes = setViewModel.Notes;
            workoutSetResult.RepsCompleted = setViewModel.RepsCompleted;
            workoutSetResult.Weight = setViewModel.Weight;
            workoutSetResult.WorkoutSetId = setViewModel.WorkoutSet.WorkoutSetId;
            workoutSetResult.WorkoutSessionId = setViewModel.WorkoutSessionId;
            workoutSetResult.Skipped = setViewModel.Skipped;

            if(workoutSetResult.Skipped)
            {
                workoutSetResult.Difficulty = 0;
                workoutSetResult.RepsCompleted = 0;
                workoutSetResult.Weight = 0;
            }

            _context.WorkoutSetResult.Add(workoutSetResult);

            _context.SaveChanges();

            return RedirectToAction("StartSet",
                new
                {
                    workoutId = setViewModel.WorkoutId,
                    workoutSetId = setViewModel.WorkoutSet.WorkoutSetId,
                    lastSet = setViewModel.IsLastSet,
                    currentExerciseId = setViewModel.Exercise.ExerciseId,
                    workoutSessionId = setViewModel.WorkoutSessionId
                });
        }


        // GET: Workout/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        public ActionResult EditWorkout([FromRoute] int id)
        {
            Workout.Models.Workout workout = _context.Workout
                .Where(w => w.WorkoutId == id)
                .Single();
            //*******************************************

            //var workouts = _context.Workout.ToList();

            var exercises = _context.Exercise.ToList();

            workout.WorkoutToExercise = _context.WorkoutToExercise.Where(we => we.WorkoutId == workout.WorkoutId && we.Active).ToList();

            foreach (var we in workout.WorkoutToExercise)
            {
                we.Exercise = exercises.Where(e => e.ExerciseId == we.ExerciseId).Single();
            }

            workout.WorkoutSet = _context.WorkoutSet.Where(ws => ws.WorkoutId == workout.WorkoutId && ws.Active).OrderBy(ws => ws.SetOrder).ToList();


            WorkoutVM workoutVM = new WorkoutVM(workout);



            Dictionary<string, List<string>> exerciseToSets = new Dictionary<string, List<string>>();

            List<WorkoutToExercise> wtes = workout.WorkoutToExercise.OrderBy(x => x.Order).ToList();

            foreach (var wte in wtes)
            {
                foreach (var ws in wte.Workout.WorkoutSet.Where(ws => ws.ExerciseId == wte.ExerciseId).OrderBy(x => x.SetOrder))
                {
                    if (exerciseToSets.ContainsKey(ws.Exercise.ExerciseName))
                    {
                        exerciseToSets[ws.Exercise.ExerciseName].Add(ws.Reps.ToString());
                    }
                    else
                    {
                        exerciseToSets[ws.Exercise.ExerciseName] = new List<string> { ws.Reps.ToString() };
                    }
                }

            }

            workoutVM.ExerciseDisplay = exerciseToSets;// exerciseToSets.Select(kvp => kvp.Key + " | " + string.Join(", ", kvp.Value)).ToList();

            foreach (ExerciseVM evm in workoutVM.AllExercises)
            {
                evm.SetsAndReps = string.Join(", ", exerciseToSets[evm.ExerciseName]);

                evm.Order = wtes.Where(wte => wte.Exercise.ExerciseName == evm.ExerciseName).Single().Order;

                evm.Selected = true;

                evm.ExerciseID = wtes.Where(wte => wte.Exercise.ExerciseName == evm.ExerciseName).Single().ExerciseId;
            }

            workoutVM.AllExercises = workoutVM.AllExercises.OrderBy(e => e.Order).ToList();


            HashSet<int> exerciseIDs = workoutVM.AllExercises.Select(ex => ex.ExerciseID).ToHashSet();

            workoutVM.AllExercises.AddRange(exercises.Where(e => !exerciseIDs.Contains(e.ExerciseId)).Select(e => new ExerciseVM { ExerciseID = e.ExerciseId, ExerciseName = e.ExerciseName }));

            //*******************************************

            //workout.WorkoutToExercise = _context.WorkoutToExercise.Where(we => we.WorkoutId == workout.WorkoutId).ToList();
            //workout.WorkoutSet = _context.WorkoutSet.Where(ws => ws.WorkoutId == workout.WorkoutId).ToList();

            //foreach (var we in workout.WorkoutToExercise)
            //{
            //    we.Exercise = _context.Exercise.Where(e => e.ExerciseId == we.ExerciseId).Single();
            //}


            //var vm = new WorkoutVM();

            //vm.Workout = workout;
            //vm.AllExercises = workout.WorkoutSet.Select(ws => ws.Exercise)
            //    .ToList().Select(x => new ExerciseVM { ExerciseID = x.ExerciseId, ExerciseName = x.ExerciseName })
            //    .ToList();

            //foreach (var ws in workout.WorkoutSet)
            //{

            //}

            return View("Edit", workoutVM);
        }

        [HttpPost]
        public ActionResult EditWorkout(WorkoutVM workoutVM)
        {
            Workout.Models.Workout workout = workoutVM.Workout;

            workout.WorkoutSet = _context.WorkoutSet.Where(ws => ws.WorkoutId == workout.WorkoutId).ToList();

            workout.WorkoutToExercise = _context.WorkoutToExercise.Where(wte => wte.WorkoutId == workout.WorkoutId).ToList();

            foreach (var wte in workout.WorkoutToExercise)
            {
                wte.Active = false;
            }

            foreach (var ws in workout.WorkoutSet)
            {
                ws.Active = false;
            }

            _context.WorkoutSet.UpdateRange(workout.WorkoutSet);
            _context.WorkoutToExercise.UpdateRange(workout.WorkoutToExercise);

            _context.SaveChanges();

            workoutVM.Workout.WorkoutToExercise = workoutVM.AllExercises
                .Where(e => e.Selected)
                .Select(x => new WorkoutToExercise { Workout = workout, ExerciseId = x.ExerciseID, Order = x.Order, Active = true })
                .ToList();

            workoutVM.AllExercises = workoutVM.AllExercises.Where(e => e.Selected).ToList();

            workoutVM.Workout.WorkoutSet = GetWorkOutSets(workoutVM, workoutVM.Workout);

            _context.Workout.Update(workout);

            _context.SaveChanges();

            return RedirectToAction("Index");

            //comment -
        }


        // POST: Workout/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Workout/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Workout/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}