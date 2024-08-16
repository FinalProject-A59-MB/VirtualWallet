namespace ForumProject.Exceptions
{
    public class DuplicateEntityException : Exception
    {
        public DuplicateEntityException(string message) : base(message)
        {
        }
    }
}
