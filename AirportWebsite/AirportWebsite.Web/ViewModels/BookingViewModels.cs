using System.ComponentModel.DataAnnotations;

namespace AirportWebsite.Web.ViewModels;

public class CreateBookingViewModel
{
    public int FlightId { get; set; }

    [Required(ErrorMessage = "Введите количество пассажиров")]
    [Range(1, 5, ErrorMessage = "Максимум 5 пассажиров за раз")]
    [Display(Name = "Количество пассажиров")]
    public int PassengerCount { get; set; } = 1;

    [Required(ErrorMessage = "Введите имя")]
    [Display(Name = "Имя")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите фамилию")]
    [Display(Name = "Фамилия")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите номер паспорта")]
    [Display(Name = "Номер паспорта")]
    [StringLength(20, MinimumLength = 4, ErrorMessage = "Некорректный номер паспорта")]
    public string PassportNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Выберите страну")]
    [Display(Name = "Страна выдачи паспорта")]
    public string PassportCountry { get; set; } = "RU";

    [Display(Name = "Дата рождения")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Телефон")]
    [Phone(ErrorMessage = "Некорректный номер телефона")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Введите номер карты")]
    [Display(Name = "Номер карты")]
    [StringLength(16, MinimumLength = 16, ErrorMessage = "Номер карты должен содержать 16 цифр")]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Только цифры, 16 знаков")]
    public string CardNumber { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }
    public string FlightInfo { get; set; } = string.Empty;
}

public class BookingResultViewModel
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? BookingReference { get; set; }
    public string? FlightNumber { get; set; }
    public string? PassengerName { get; set; }
    public DateTime FlightDate { get; set; }
}

public class UserBookingsViewModel
{
    public int Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}
