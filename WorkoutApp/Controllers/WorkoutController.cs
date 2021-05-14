using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Models;
using WorkoutApp.ViewModels;

namespace WorkoutApp.Controllers
{
    public class WorkoutController : Controller
    {
        WorkoutContext _context;

        public WorkoutController(WorkoutContext workoutContext)
        {
            _context = workoutContext;
        }

        // GET: Workout
        public ActionResult Index()
        {
            var workouts = _context.Workout.ToList();

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

                workoutVM.ExerciseDisplay = exerciseToSets.Select(kvp => kvp.Key + " | " + string.Join(", ", kvp.Value)).ToList();

                workoutViewModels.Add(workoutVM);
            }

            return View(workoutViewModels);
        }

        [Route("{workoutId:int}")]
        public ActionResult ViewHistory([FromRoute] int workoutId)
        {
            var workout = _context.Workout.Where(w => w.WorkoutId == workoutId).First();

            var workoutToExercise = _context.WorkoutToExercise.Where(wte => wte.WorkoutId == workoutId && wte.Active).ToList();

            var exercises = _context.Exercise.Where(e => workoutToExercise.Select(x => x.ExerciseId).Contains(e.ExerciseId)).ToList();

            var workoutSets = _context.WorkoutSet.Where(ws => ws.WorkoutId == workoutId && ws.Active).ToList();

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
                        var resultLine = $"Set {i}: Weight = {result.Weight}. Reps = {result.RepsCompleted}.";

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

            //WorkoutHistoryVM workoutHistoryVM = new WorkoutHistoryVM();

            //workoutHistoryVM.ExerciseHistories = new Dictionary<string, List<WorkoutSetResult>>();

            //foreach (WorkoutSetResult wsr in workoutSetResults)
            //{
            //    WorkoutSet workoutSet = workoutSets.Where(x => x.WorkoutSetId == wsr.WorkoutSetId).First();

            //    string exerciseName = exercises.Where(e => e.ExerciseId == workoutSet.ExerciseId).First().ExerciseName;

            //    if (!workoutHistoryVM.ExerciseHistories.ContainsKey(exerciseName))
            //    {
            //        workoutHistoryVM.ExerciseHistories[exerciseName] = new List<WorkoutSetResult> { wsr };
            //    }
            //    else
            //    {
            //        workoutHistoryVM.ExerciseHistories[exerciseName].Add(wsr);
            //    }
            //}

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

            Workout workout = _context.Workout.Find(id);

            //if (student == null)
            //{
            //    return HttpNotFound();
            //}


            return View(workout);
        }

        // GET: Workout/Create
        public ActionResult Create()
        {
            var vm = new WorkoutVM();

            vm.Workout = new Workout();
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
        public ActionResult Create([FromForm] WorkoutVM workoutVM)
        {
            var workout = new Workout();

            workout.DateCreated = DateTime.Now;
            workout.CreatedBy = 1;
            workout.WorkoutName = workoutVM.Workout.WorkoutName;
            workout.WorkoutToExercise = workoutVM.AllExercises.Where(e => e.Selected).Select(x => new WorkoutToExercise { Workout = workout, ExerciseId = x.ExerciseID, Order = x.Order, Active = true }).ToList();
            workoutVM.AllExercises = workoutVM.AllExercises.Where(e => e.Selected).ToList();

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

        public List<WorkoutSet> GetWorkOutSets(WorkoutVM workoutVM, Workout workout)
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

        [Route("{workoutId:int}/{workoutSetId:int}/{lastSet:bool}/{currentExerciseId:int}/{workoutSessionId:int}")]
        public ActionResult StartSet([FromRoute] int workoutId, int workoutSetId, bool lastSet, int currentExerciseId, int workoutSessionId)
        {
            if(workoutSessionId == 0)
            {
                WorkoutSession workoutSession = new WorkoutSession { DateCreated = DateTime.Now };

                _context.WorkoutSession.Add(workoutSession);

                _context.SaveChanges();

                workoutSessionId = workoutSession.WorkoutSessionId;
            }

            

            int exerciseId = 0;

            Exercise exercise = null;

            List<WorkoutToExercise> wtes = _context.WorkoutToExercise
                .Where(we => we.WorkoutId == workoutId && we.Active)
                .OrderBy(wte => wte.Order).ToList();

            List<Exercise> exercises = _context.Exercise
                .Where(e => wtes.Select(wte => wte.ExerciseId)
                .Contains(e.ExerciseId)).ToList();

            bool workoutComplete = false;

            if (workoutSetId == 0) //firstSet
            {
                exerciseId = wtes.OrderBy(x => x.Order).First().ExerciseId;
            }
            else if (lastSet) //last set -- need next exercise
            {
                int currentOrder = wtes.Where(wte => wte.ExerciseId == currentExerciseId).First().Order;

                WorkoutToExercise workoutToExercise = wtes.Where(wte => wte.Order == currentOrder + 1).FirstOrDefault();

                if (workoutToExercise == null)
                {
                    workoutComplete = true;
                }
                else
                {
                    exerciseId = wtes.Where(wte => wte.Order == currentOrder + 1).FirstOrDefault().ExerciseId;//is null posible here?
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

            var workoutSets = _context.WorkoutSet
                .Where(ws => ws.WorkoutId == workoutId && ws.ExerciseId == exerciseId && ws.Active)
                .OrderBy(x => x.SetOrder).ToList();

            WorkoutSet currentWorkoutSet = null;

            bool nextSetIsLast = false;

            WorkoutSet nextWorkoutSet = null;

            SetViewModel setViewModel = null;

            WorkoutSetResult previousResult = null;

            if (workoutSetId == 0 || lastSet)
            {
                workoutSetId = workoutSets.Where(ws => ws.ExerciseId == exercise.ExerciseId).OrderBy(ws => ws.SetOrder).First().WorkoutSetId;

                nextWorkoutSet = workoutSets.Where(ws => ws.WorkoutSetId == workoutSetId).First();

                nextSetIsLast = nextWorkoutSet.SetOrder == workoutSets.Select(ws => ws.SetOrder).Max();

                previousResult = GetLastWorkoutSetResult(nextWorkoutSet.WorkoutSetId);

                if(workoutSessionId == 0 && (DateTime.Now - previousResult.DateCreated).TotalMinutes < 60)
                {
                    workoutSessionId = previousResult.WorkoutSessionId;
                }

                setViewModel = new SetViewModel(previousResult);

                setViewModel.Exercise = exercise;
                setViewModel.WorkoutSet = nextWorkoutSet;
                setViewModel.WorkoutId = workoutId;
                setViewModel.IsLastSet = nextSetIsLast;
                setViewModel.PreviousWorkoutSetResult = previousResult;

                ModelState.SetModelValue("workoutSessionId", new ValueProviderResult(new Microsoft.Extensions.Primitives.StringValues(workoutSessionId.ToString()), CultureInfo.InvariantCulture));

                return View("Set", setViewModel);
            }

            currentWorkoutSet = workoutSets.Where(ws => ws.WorkoutSetId == workoutSetId).First();

            nextWorkoutSet = workoutSets.Where(ws => ws.ExerciseId == exerciseId && ws.SetOrder > currentWorkoutSet.SetOrder).First();

            nextSetIsLast = nextWorkoutSet.SetOrder == workoutSets.Select(ws => ws.SetOrder).Max();

            previousResult = GetLastWorkoutSetResult(nextWorkoutSet.WorkoutSetId);

            if (workoutSessionId == 0 && (DateTime.Now - previousResult.DateCreated).TotalMinutes < 60)
            {
                workoutSessionId = previousResult.WorkoutSessionId;
            }

            setViewModel = new SetViewModel(previousResult);

            setViewModel.Exercise = exercise;
            setViewModel.WorkoutSet = nextWorkoutSet;
            setViewModel.WorkoutId = workoutId;
            setViewModel.IsLastSet = nextSetIsLast;
            setViewModel.PreviousWorkoutSetResult = previousResult;

            ModelState.SetModelValue("workoutSessionId", new ValueProviderResult(new Microsoft.Extensions.Primitives.StringValues(workoutSessionId.ToString()), CultureInfo.InvariantCulture));

            return View("Set", setViewModel);
        }


        public WorkoutSetResult GetLastWorkoutSetResult(int workoutSetId)
        {
            var lastWorkoutSetResult = _context.WorkoutSetResult.Where(wsr => wsr.WorkoutSetId == workoutSetId)
                                             .OrderByDescending(x => x.DateCreated).FirstOrDefault();

            return lastWorkoutSetResult;
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

            _context.WorkoutSetResult.Add(workoutSetResult);

            _context.SaveChanges();

            return RedirectToAction("StartSet", 
                new { workoutId = setViewModel.WorkoutId,
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
            Workout workout = _context.Workout
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

            workoutVM.ExerciseDisplay = exerciseToSets.Select(kvp => kvp.Key + " | " + string.Join(", ", kvp.Value)).ToList();

            foreach (ExerciseVM evm in workoutVM.AllExercises)
            {
                evm.SetsAndReps = string.Join(", ", exerciseToSets[evm.ExerciseName]);

                evm.Order = wtes.Where(wte => wte.Exercise.ExerciseName == evm.ExerciseName).Single().Order;

                evm.Selected = true;

                evm.ExerciseID = wtes.Where(wte => wte.Exercise.ExerciseName == evm.ExerciseName).Single().ExerciseId;
            }

            workoutVM.AllExercises = workoutVM.AllExercises.OrderBy(e => e.Order).ToList();


            HashSet<int> exerciseIDs = workoutVM.AllExercises.Select(ex => ex.ExerciseID).ToHashSet();

            workoutVM.AllExercises.AddRange(exercises.Where(e => !exerciseIDs.Contains(e.ExerciseId)).Select(e => new ExerciseVM { ExerciseID = e.ExerciseId, ExerciseName = e.ExerciseName}));

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
            Workout workout = workoutVM.Workout;

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