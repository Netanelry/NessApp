namespace NessServer.Exceptions
{
    public class DuplicateIdException : Exception
    {
        public DuplicateIdException(int id)
            : base($"Duplicate ID found: {id}")
        {
        }
    }

    public class ClientAlreadyExistsException : Exception
    {
        public ClientAlreadyExistsException(int id)
            : base($"Client with ID {id} already exists in the database.")
        {
        }
    }
}
