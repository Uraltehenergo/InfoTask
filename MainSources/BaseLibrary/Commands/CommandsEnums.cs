namespace BaseLibrary
{
    //Были ли ошибки, при выполнении комманды
    public enum CommandQuality : byte
    {
        Success = 1,
        Warning = 2,
        Error = 3
    }

    //---------------------------------------------------------------------------------------
    //Дополнительная функция комманды
    public enum CommandBehaviour
    {
        Simple, //Просто комманда
        Log, //Комманда для записи в History
        SubLog, //Комманда для записи в SubHistory 
        Danger //Комманда обрамляющая опасную операцию
    }
}
