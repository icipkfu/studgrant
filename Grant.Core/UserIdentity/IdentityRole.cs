﻿using Microsoft.AspNet.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspNet.Identity.PostgreSQL
{
    [Table("AspNetRoles", Schema = "grant2")]
    public class IdentityRole : IRole
    {
        /// <summary>
        /// Default constructor for IdentityRole. 
        /// </summary>
        public IdentityRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// Constructor that takes a name as an argument.
        /// </summary>
        /// <param name="name"></param>
        public IdentityRole(string name)
            : this()
        {
            this.Name = name;
        }

        public IdentityRole(string name, string id)
        {
            this.Name = name;
            this.Id = id;
        }

        /// <summary>
        /// Role ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Role name.
        /// </summary>
        public string Name { get; set; }
    }
}
