using fitnessCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace fitnessCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private FitnessContext f_db = new FitnessContext();
        // GET: api/<ExerciseController>
        [HttpGet]
        public List<Exercise> Get()
        {
            var exercises = f_db.exercises.ToList();
            return exercises;
        }

        // GET api/<ExerciseController>/5
        [HttpGet("{id}")]
        public ActionResult<Exercise> Get(int id)
        {
            var exercise = f_db.exercises.FirstOrDefault(x => x.exId == id);
            if (exercise is null)
            {
                return NoContent();
            }
            return exercise;
        }

        // POST api/<ExerciseController>
        [HttpPost]
        public ActionResult Post([FromBody] Exercise ex)
        {
            f_db.exercises.Add(ex);
            f_db.SaveChanges();
            return Ok("Exercise Added");
        }

        // PUT api/<ExerciseController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Exercise ex)
        {
            Exercise exercise = f_db.exercises.FirstOrDefault(x => x.exId == id);
            if (exercise is null)
            {

                return NotFound();
            }
            exercise.exerciseType = ex.exerciseType;
            exercise.Price = ex.Price;
            f_db.exercises.Update(exercise);
            f_db.SaveChanges();
            return Ok("Exercise updated");
        }

        // DELETE api/<ExerciseController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var exercise = f_db.exercises.Include(x => x.availCotchs).FirstOrDefault(x => x.exId == id);
            if (exercise is null)
            {
                return NoContent();
            }
            if (exercise.availCotchs != null && exercise.availCotchs.Count > 0)
            {
                return NoContent();
            }
            f_db.Remove(exercise);
            f_db.SaveChanges();
            return Ok("Exercise Deleted");
        }
    }
}
