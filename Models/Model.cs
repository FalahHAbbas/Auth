using System;
using System.ComponentModel.DataAnnotations;

namespace Auth.Models {
    public class Model<TId> {
        [Key]
        public TId Id { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}