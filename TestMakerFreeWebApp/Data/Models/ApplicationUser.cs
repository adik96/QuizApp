using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TestMakerFreeWebApp.Data
{
    public class ApplicationUser
    {
        #region Konstruktor
        public ApplicationUser()
        {
        }
        #endregion

        #region Właściwości
        [Key]
        [Required]
        public string Id { get; set; }
        [Required]
        [MaxLength(128)]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Notes { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public int Flags { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime LastModifiedDate { get; set; }
        #endregion

        #region Właściwości wczytywane leniwie
        /// <summary>
        /// Lista wszystkich quizów utworzonych przez tego użytkownika.
        /// </summary>
        public virtual List<Quiz> Quizzes { get; set; }
        #endregion
    }
}