namespace CommonTypes
{
    //Один сигнал для чтения по блокам
    public class ProviderObject
    {
        public ProviderObject()
        {
        }

        public ProviderObject(string inf)
        {
            Inf = inf;
        }

        //Информация по сигналу
        public string Inf { get; set; }
    }
}