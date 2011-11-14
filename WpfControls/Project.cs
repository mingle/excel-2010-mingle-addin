/*
 * (c) 2011 ThoughtWorks, Inc.
 */

using System;
using System.Diagnostics;

namespace WpfControls
{
    /// <summary>
    /// Project class
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Project name
        /// </summary>
        public String Name { get; private set; }
        /// <summary>
        /// Project Id
        /// </summary>
        public String Id { get; private set; }

        /// <summary>
        /// Constructs a project
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public Project(string id, string name)
        {
            Debug.Assert(!String.IsNullOrEmpty(id), "id = null");
            Debug.Assert(!String.IsNullOrEmpty(name), "id = name");
            Id = id;
            Name = name;
        }

        /// <summary>
        /// True if the project has the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasName(string name)
        {
            return Name.Equals(name);
        }

        public bool HasId(string id)
        {
            return Id == id;
        }

        public override string ToString()
        {
            return Id + ":" + Name;
        }

        public bool Equals(Project other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && Equals(other.Id, Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Project)) return false;
            return Equals((Project) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ (Id != null ? Id.GetHashCode() : 0);
            }
        }
    }
}