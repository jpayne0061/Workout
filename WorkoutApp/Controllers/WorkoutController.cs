using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            workout.WorkoutToExercise = workoutVM.AllExercises.Where(e => e.Selected).Select(x => new WorkoutToExercise { Workout = workout, ExerciseId = x.ExerciseID, Order = x.Order, Active = true}).ToList();
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

                    //if(edit)
                    //{
                    //    if (workout.WorkoutToExercise.Select(e => e.ExerciseId).Contains(exercise.ExerciseID))
                    //    {
                    //        workoutSet.WorkoutSetId = workout.WorkoutSet.Where(ws => ws.ExerciseId == exercise.ExerciseID).First().WorkoutSetId;
                    //    }
                    //}
                    
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

        [Route("{id:int}")]
        public ActionResult Start([FromRoute] int id)
        {
            Workout workout = _context.Workout.Where(w => w.WorkoutId == id).Single();

            int firstExerciseID = _context.WorkoutToExercise.Where(we => we.WorkoutId == id).OrderBy(x => x.Order).First().ExerciseId;

            Exercise firstExercise = _context.Exercise.Where(e => e.ExerciseId == firstExerciseID).Single();

            var firstSet = _context.WorkoutSet.Where(ws => ws.ExerciseId == firstExerciseID).OrderBy(x => x.SetOrder).First();

            SetViewModel setViewModel = new SetViewModel { Exercise = firstExercise, WorkoutSet = firstSet };

            return View("Set", setViewModel);
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

            _context.WorkoutSetResult.Add(workoutSetResult);

            _context.SaveChanges();

            //return View next set or next exercise

            return View("Set", setViewModel);
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

            workout.WorkoutSet = _context.WorkoutSet.Where(ws => ws.WorkoutId == workout.WorkoutId).OrderBy(ws => ws.SetOrder).ToList();


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

            workoutVM.Workout.WorkoutSet = GetWorkOutSets(workoutVM, workoutVM.Workout);
            
            _context.Workout.Update(workout);

            _context.SaveChanges();

            return RedirectToAction("Index");
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