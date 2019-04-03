using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TestMakerFreeWebApp.Data
{
    public class Quiz
    {
        #region Konstruktor
        public Quiz()
        {
        }
        #endregion
        #region Właściwości
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }
        public string Notes { get; set; }
        [DefaultValue(0)]
        public int Type { get; set; }
        [DefaultValue(0)]
        public int Flags { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int ViewCount { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime LastModifiedDate { get; set; }
        #endregion

        #region Właściwości wczytywane leniwie
        /// <summary>
        /// Autor quizu: będzie wczytywany przy pierwszym
        /// użyciu dzięki funkcjonalności leniwego wczytywania EF.
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        /// <summary>
        /// Lista wszystkich pytań powiązanych z quizem.
        /// Zostanie wypełniona przy pierwszym użyciu dzięki funkcjonalności leniwego wczytywania EF.
        /// </summary>
        public virtual List<Question> Questions { get; set; }
        /// <summary>
        /// Lista wszystkich wyników powiązanych z quizem.
        /// Zostanie wypełniona przy pierwszym użyciu dzięki funkcjonalności leniwego wczytywania EF.
        /// </summary>
        public virtual List<Result> Results { get; set; }
        #endregion
    }
}