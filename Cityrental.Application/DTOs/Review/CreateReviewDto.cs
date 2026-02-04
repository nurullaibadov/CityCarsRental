using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.DTOs.Review
{
    public class CreateReviewDto
    {
        public Guid RentalId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
