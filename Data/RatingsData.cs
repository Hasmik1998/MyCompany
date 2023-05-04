using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chess_Up.Data
{
    public class RatingsData
    {
        public int ID { get; set; }

        public string Full_Name { get; set; }

        public string Sex { get; set; }

        public string Region { get; set; }

        public string School_Name { get; set; }

        public string Trainer { get; set; }

        public System.Int16 rating_standard_2016 { get; set; }

        public System.Int16 rating_standard_2017 { get; set; }

        public System.Int16 rating_standard_2018 { get; set; }

        public System.Int16 rating_standard_2019 { get; set; }

        public double rating_standard_2020 { get; set; }

        public double rating_standard_2021 { get; set; }

        public double rating_standard_2022 { get; set; }
    }
}
