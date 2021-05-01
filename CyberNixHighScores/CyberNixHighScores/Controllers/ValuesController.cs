using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CyberNixHighScores.Controllers
{
    public class ScoreDTO
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }

    public class HighScoresDTO
    {
        public List<ScoreDTO> Scores { get; set; }
    }



    [ApiController]
    public class HomeController : ControllerBase
    {
        private static object _lock = new object();
        private static string _filename = "Scores.json";

        [Route("getscores")]
        [HttpGet]
        public ActionResult<HighScoresDTO> getscores()
        {
            lock (_lock)
            {


                try
                {
                    var scores = System.IO.File.ReadAllText(_filename);
                    var dto = JsonConvert.DeserializeObject<HighScoresDTO>(scores);
                    return dto;
                }
                catch (Exception e)
                {
                }
            }
            return null;
        }

        [Route("setscore")]
        [HttpPost]
        public ActionResult setscore([FromHeader] string name, [FromHeader] int score, [FromHeader] string pwd)
        {
            if (pwd != "trollolol")
                return BadRequest();
            lock (_lock)
            {
                try
                {
                    HighScoresDTO dto = null;
                    if (System.IO.File.Exists(_filename))
                    {
                        var scores = System.IO.File.ReadAllText(_filename);
                        dto = JsonConvert.DeserializeObject<HighScoresDTO>(scores);
                    }
                    else
                    {
                        dto = new HighScoresDTO();
                        dto.Scores = new List<ScoreDTO>();
                    }

                    dto.Scores.Sort((a, b) => { return b.Score - a.Score; });

                    int newIndex = dto.Scores.FindIndex((el) =>
                    {
                        return score > el.Score;
                    });
                    int limit = 100;

                    if (newIndex < limit)
                    {
                        newIndex = Math.Max(newIndex, 0);
                        ScoreDTO newDto = new ScoreDTO();
                        newDto.Name = name;
                        newDto.Score = score;
                        dto.Scores.Insert(newIndex, newDto);
                        dto.Scores = dto.Scores.Take(limit).ToList();

                        System.IO.File.WriteAllText(_filename, JsonConvert.SerializeObject(dto));
                    }

                }
                catch (Exception e) { }
                return Ok();
            }

        }
    }
}
