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
    public class QuestionController : BaseApiController
    {
        #region Konstruktor
        public QuestionController(ApplicationDbContext context)
            : base(context) { }
        #endregion

        // GET api/question/all
        [HttpGet("All/{quizId}")]
        public IActionResult All(int quizId)
        {
            var questions = DbContext.Questions
                .Where(q => q.QuizId == quizId)
                .ToArray();

            return new JsonResult(
            questions.Adapt<QuestionViewModel[]>(),
            JsonSettings);
        }
        #region Metody dostosowujące do konwencji REST
        /// <summary>
        /// Pobiera pytanie o podanym {id}
        /// </summary>
        /// <param name="id">identyfikator istniejącego pytania</param>
        /// <returns>pytanie o podanym {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var question = DbContext.Questions.Where(i => i.Id == id)
                .FirstOrDefault();

            // Obsłuż żądania proszące o nieistniejące pytania
            if (question == null) return NotFound(new
            {
                Error = String.Format("Nie znaleziono pytania o identyfikatorze { 0}", id)});
        return new JsonResult(
            question.Adapt<QuestionViewModel>(),
            JsonSettings);
        }
        /// <summary>
        /// Dodaje nowe pytanie do bazy danych
        /// </summary>
        /// <param name="model">obiekt QuestionViewModel z danymi do wstawienia</param>
        [HttpPut]
        public IActionResult Put([FromBody]QuestionViewModel model)
        {
            // Zwraca ogólny kod statusu HTTP 500 (Server Error),
            // jeśli dane przesłane przez klienta są niewłaściwe
            if (model == null) return new StatusCodeResult(500);
            // Odwzorowuje ViewModel na Model
            var question = model.Adapt<Question>();
            // Nadpisz właściwości,
            // które powinny być ustawiane tylko przez serwer
            question.CreatedDate = DateTime.Now;
            question.LastModifiedDate = question.CreatedDate;
            // Dodaj nowe pytanie
            DbContext.Questions.Add(question);
            // Zapisz zmiany w bazie danych
            DbContext.SaveChanges();
            // Zwróć nowo dodane pytanie klientowi
            return new JsonResult(question.Adapt<QuestionViewModel>(),
            JsonSettings);
        }
        /// <summary>
        /// Modyfikuje pytanie o podanym {id}
        /// </summary>
        /// <param name="model">obiekt QuestionViewModel z danymi do uaktualnienia</param>
        [HttpPost]
        public IActionResult Post([FromBody]QuestionViewModel model)
        {
            // Zwraca ogólny kod statusu HTTP 500 (Server Error),
            // jeśli dane przesłane przez klienta są niewłaściwe
            if (model == null) return new StatusCodeResult(500);
            // Pobierz pytanie do edycji
            var question = DbContext.Questions.Where(q => q.Id ==
            model.Id).FirstOrDefault();
            // Obsłuż żądania proszące o nieistniejące pytania
            if (question == null) return NotFound(new
            {
                Error = String.Format("Nie znaleziono pytania o identyfikatorze {0}",
            model.Id)
            });
            // Obsłuż aktualizację (bez odwzorowania obiektów)
            // za pomocą ręcznego przepisania właściwości
            // otrzymanych w żądaniu od klienta
            question.QuizId = model.QuizId;
            question.Text = model.Text;
            question.Notes = model.Notes;
            // Właściwości ustawiane tylko przez serwer
            question.LastModifiedDate = DateTime.Now;
            // Zapisz zmiany w bazie danych
            DbContext.SaveChanges();
            // Zwróć uaktualnione pytanie klientowi
            return new JsonResult(question.Adapt<QuestionViewModel>(),
            JsonSettings);
        }
        /// <summary>
        /// Usuwa pytanie o podanym {id} z bazy danych
        /// </summary>
        /// <param name="id">identyfikator istniejącego pytania</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Pobierz pytanie z bazy danych
            var question = DbContext.Questions.Where(i => i.Id == id)
            .FirstOrDefault();

            // Obsłuż żądania proszące o nieistniejące pytania
            if (question == null)
                return NotFound(new
                {
                    Error = String.Format("Nie znaleziono pytania o identyfikatorze { 0}", id)
                }
            );
            // Usuń pytanie z bazy danych
            DbContext.Questions.Remove(question);
            // Zapisz zmiany w bazie danych
            DbContext.SaveChanges();
            // Zwróć kod statusu HTTP 204
            return new NoContentResult();
        }
        #endregion
    }
}