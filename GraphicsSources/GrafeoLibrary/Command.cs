using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrafeoLibrary
{
    internal abstract class Command
    {
        public object Sender { get; protected set; }
        public string Description { get; protected set; }
        
        protected Command(object sender, string description)
        {
            Sender = sender;
            Description = description;
        }

        public abstract void Do();
        public abstract void Undo();
    }

    internal class MacroCommand : Command
    {
        private readonly List<Command> _commands;

        public MacroCommand(object sender, List<Command> commands, string description) : base(sender, description)
        {
            _commands = commands;
        }
        
        public override void Do()
        {
            foreach (var command in _commands)
                command.Do();
        }

        public override void Undo()
        {
            for (var i = _commands.Count - 1; i >= 0; i--)
                _commands[i].Undo();
        }
    }

    internal class CommandSetViewTimeBegin : Command
    {
        private readonly DateTime _newViewTimeBegin;
        private readonly DateTime _oldViewTimeBegin;

        public CommandSetViewTimeBegin(object sender, DateTime newViewTimeBegin, DateTime curViewTimeBegin) : base(sender, "Time Begin")
        {
            _newViewTimeBegin = newViewTimeBegin;
            _oldViewTimeBegin = curViewTimeBegin;
        }

        public override void Do()
        {
            //Commander.ViewTimeBegin = _newViewTimeBegin;
        }

        public override void Undo()
        {
            //Commander.ViewTimeBegin = _oldViewTimeBegin;
        }
    }

    internal class CommandSetViewTimeEnd : Command
    {
        private readonly DateTime _newViewTimeEnd;
        private readonly DateTime _oldViewTimeEnd;

        public CommandSetViewTimeEnd(object sender, DateTime newViewTimeEnd, DateTime curViewTimeEnd) : base(sender, "Time End")
        {
            _newViewTimeEnd = newViewTimeEnd;
            _oldViewTimeEnd = curViewTimeEnd;
        }

        public override void Do()
        {
            //Receiver.ViewTimeEnd = _newViewTimeEnd;
        }

        public override void Undo()
        {
            //Receiver.ViewTimeEnd = _oldViewTimeEnd;
        }
    }
}
