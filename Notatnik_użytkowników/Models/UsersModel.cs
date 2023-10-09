using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notatnik_użytkowników.Models
{
    public class UsersModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Pole Imie wymagane")]
        [Display(Name = "Imie")]
        [StringLength(50, ErrorMessage = "Maksymalna długość do 50 znaków")]
        [RegularExpression(@"^[A-Za-z-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$", ErrorMessage = "Pole Imie moze zawierać tylko litery")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Pole Nazwisko wymagane")]
        [Display(Name = "Nazwisko")]
        [StringLength(150, ErrorMessage = "Maksymalna długość do 150 znaków")]
        [RegularExpression(@"^[A-Za-z-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$", ErrorMessage = "Pole Nazwisko moze zawierać tylko litery")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Pole Data urodzenia wymagane")]
        [Display(Name = "Data urodzenia")]
        [DataType(DataType.Date)]
        public DateTime Birth { get; set; }


        [Display(Name = "Płeć")]
        [Required(ErrorMessage = "Pole Płeć jest wymagane.")]
        public string SelectedGender { get; set; }

        public List<SelectListItem> GenderOptions { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Mężyczyzna", Text = "Mężczyzna" },
            new SelectListItem { Value = "Kobieta", Text = "Kobieta" },
        };
        [Display(Name= "Dodatkowe atrybuty")]
        public List<(string,string)> Attributes { get; set; }
    }
}
