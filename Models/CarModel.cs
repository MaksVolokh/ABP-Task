
namespace ABP_Task.Models
{
    internal class CarModel
    {
        public string CarMake { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DateRange { get; set; }
        public string Complectation { get; set; }

        public override string ToString()
        {
            return $"{Name}\n{Code} {DateRange} {Complectation}\n";
        }
    }
}
