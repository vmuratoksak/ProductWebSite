using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SatisSitesi.Domain.Entities
{
    public class NameEntity : BaseEntity
    {
        [Required(ErrorMessage = "Isim zorunludur.")]
        [StringLength(50, ErrorMessage = "Isim en fazla 50 karakter olabilir.")]
        public string Name { get; set; }
    }
}
