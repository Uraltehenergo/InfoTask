namespace BaseLibrary
{
    public class CommandDanger : Command
    {
        internal CommandDanger(Logger logger, Command parent) 
            : base(logger, parent, 0, 100, "")
        {
        }

        //Добавить в комманду ошибку error, isRepeated - ошибка произошла в повторяемой операции
        public override void AddError(ErrorCommand error, bool isRepeated)
        {
            ChangeQuality(error, isRepeated);
            if (Parent != null) Parent.AddError(error, true);
        }
    }
}