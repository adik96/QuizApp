using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestMakerFreeWebApp.ViewModels;
using System.Collections.Generic;
using System.Linq;
using TestMakerFreeWebApp.Data;
using Mapster;

namespace TestMakerFreeWebApp.Controllers
{
    public class QuizController : BaseApiController
    {
        #region Konstruktor
        public QuizController(ApplicationDbContext context)
            : base(context) { }
        #endregion

        #region Metody dostosowujące do konwencji REST
        /// <summary>
        /// GET: api/quiz/{}id
        /// Pobiera quiz o podanym {id}
        /// </summary>
        /// <param name="id">Identyfikator istniejącego quizu</param>
        /// <returns>Quiz o podanym {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var quiz = DbContext.Quizzes.Where(i => i.Id == id).FirstOrDefault();

            // Obsłuż żądania proszące o nieistniejące quizy
            if (quiz == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Nie znaleziono quizu o identyfikatorze {0}", id)
                });
            }

            return new JsonResult(
            quiz.Adapt<QuizViewModel>(),
            JsonSettings);
        }

        /// <summary>
        /// Dodaje nowy quiz do bazy danych
        /// </summary>
        /// <param name="model">obiekt QuizViewModel z danymi do wstawienia</param>
        [HttpPut]
        public IActionResult Put([FromBody]QuizViewModel model)
        {
            // Zwraca ogólny kod statusu HTTP 500 (Server Error),
            // jeśli dane przesłane przez klienta są niewłaściwe
            if (model == null) return new StatusCodeResult(500);
            // Obsługa wstawienia (bez odwzorowania obiektów)
            var quiz = new Quiz();
            // Właściwości pobierane z żądania
            quiz.Title = model.Title;
            quiz.Description = model.Description;
            quiz.Text = model.Text;
            quiz.Notes = model.Notes;
            // Właściwości ustawiane tylko przez serwer
            quiz.CreatedDate = DateTime.Now;
            quiz.LastModifiedDate = quiz.CreatedDate;
            // Tymczasowo ustaw autora na użytkownika administracyjnego,
            // bo logowanie nie jest jeszcze obsługiwane. Zmienimy to w przyszłości
            quiz.UserId = DbContext.Users.Where(u => u.UserName == "Admin")
            .FirstOrDefault().Id;
            // Dodaj nowy quiz
            DbContext.Quizzes.Add(quiz);
            // Zapisz zmiany w bazie danych
            DbContext.SaveChanges();
            // Zwróć nowo utworzony quiz do klienta
            return new JsonResult(quiz.Adapt<QuizViewModel>(),
            JsonSettings);
        }
        /// <summary>
        /// Modyfikuje quiz o podanym {id}
        /// </summary>
        /// <param name="model">obiekt QuizViewModel z danymi do uaktualnienia</param>
        [HttpPost]
        public IActionResult Post([FromBody]QuizViewModel model)
        {
            // Zwraca ogólny kod statusu HTTP 500 (Server Error),
            // jeśli dane przesłane przez klienta są niewłaściwe
            if (model == null) return new StatusCodeResult(500);
            // Pobierz quiz do zmodyfikowania
            var quiz = DbContext.Quizzes.Where(q => q.Id ==
            model.Id).FirstOrDefault();
            // Obsłuż żądania proszące o nieistniejące quizy
            if (quiz == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Nie znaleziono quizu o identyfikatorze {0}", 
                    model.Id)
                });
            }
            // Obsłuż aktualizację (bez odwzorowania obiektów)
            // za pomocą ręcznego przepisania właściwości
            // otrzymanych w żądaniu od klienta
            quiz.Title = model.Title;
            quiz.Description = model.Description;
            quiz.Text = model.Text;
            quiz.Notes = model.Notes;
            // Właściwości ustawiane tylko przez serwer
            quiz.LastModifiedDate = DateTime.Now;
            // Zapisz zmiany w bazie danych
            DbContext.SaveChanges();
            // Zwróć klientowi zaktualizowany quiz
            return new JsonResult(quiz.Adapt<QuizViewModel>(),
            JsonSettings);
        }
        /// <summary>
        /// Usuwa quiz o podanym {id} z bazy danych
        /// </summary>
        /// <param name="id">identyfikator istniejącego quizu</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Pobierz quiz do zmodyfikowania
            var quiz = DbContext.Quizzes.Where(q => q.Id ==
            id).FirstOrDefault();
            // Obsłuż żądania proszące o nieistniejące quizy
            if (quiz == null)
            {
                return NotFound(new
                {
                    Error = String.Format("Nie znaleziono quizu o identyfikatorze {0}", id)
                });
            }
            // Usuń quiz z bazy danych
            DbContext.Quizzes.Remove(quiz);
            // Zapisz zmiany w bazie danych
            DbContext.SaveChanges();
            // Zwróć kod statusu HTTP 204
            return new NoContentResult();
        }
        #endregion
        #region Metody routingu bazujące na atrybutach
        // GET api/quiz/latest
        [HttpGet("Latest/{num?}")]
        public IActionResult Latest(int num = 10)
        {
            var latest = DbContext.Quizzes
            .OrderByDescending(q => q.CreatedDate)
            .Take(num)
            .ToArray();

            return new JsonResult(
            latest.Adapt<QuizViewModel[]>(),
            JsonSettings);
        }

        /// <summary>
        /// GET: api/quiz/ByTitle
        /// Pobiera {num} quizów posortowanych po tytule (od A do Z)
        /// </summary>
        /// <param name="num">liczba quizów do pobrania</param>
        /// <returns>{num} quizów posortowanych po tytule</returns>
        [HttpGet("ByTitle/{num:int?}")]
        public IActionResult ByTitle(int num = 10)
        {
            var byTitle = DbContext.Quizzes
            .OrderBy(q => q.Title)
            .Take(num)
            .ToArray();

            return new JsonResult(
            byTitle.Adapt<QuizViewModel[]>(),
            JsonSettings);
        }

        /// <summary>
        /// GET: api/quiz/mostViewed
        /// Pobiera {num} losowych quizów
        /// </summary>
        /// <param name="num">liczba quizów do pobrania</param>
        /// <returns>{num} losowych quizów</returns>
        [HttpGet("Random/{num:int?}")]
        public IActionResult Random(int num = 10)
        {
            var random = DbContext.Quizzes
            .OrderBy(q => Guid.NewGuid())
            .Take(num)
            .ToArray();

            return new JsonResult(
            random.Adapt<QuizViewModel[]>(),
            JsonSettings);
        }
        #endregion
    }
}