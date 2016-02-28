namespace ParturitionModel.Core
{
    internal sealed class Environment
    {
        public int Year { get; private set; }

        public void IncreaseYear()
        {
            Year += 1;
        }
    }
}