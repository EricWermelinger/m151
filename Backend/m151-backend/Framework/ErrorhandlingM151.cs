namespace m151_backend.ErrorHandling
{
    public class ErrorhandlingM151<T> where T : class
    {
        public string ErrorNotFound()
        {
            return typeof(T).Name + " not found.";
        }

        public string DataNotValid()
        {
            return "The inserted value for " + typeof(T).Name + " are not valid.";
        }
    }
}
