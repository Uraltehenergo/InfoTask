using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrafeoLibrary
{
    internal class Commander
    {
        private readonly int _maxCommandCount = -1; //Максимальное количество команд в списке (-1, неограничено)
        
        private readonly List<Command> _doCommands = new List<Command>();
        private readonly List<Command> _undoCommands = new List<Command>();

        public Commander(int maxCommandCount = 0)
        {
            //_commander = commander;
            _maxCommandCount = maxCommandCount;
        }

        internal bool CanUndo
        {
            get { return (_doCommands.Count > 0); }
        }

        internal bool CanRedo
        {
            get { return (_undoCommands.Count > 0); }
        }

        //текстовый список команд, которые можно отменить (не более count последних, -1 - все)
        internal string UndoStringList(int count = -1)
        {
            if ((_doCommands.Count > 0) && (count >= 0))
            {
                string undoList = "";
                int lastIndex = _doCommands.Count - count;
                if (lastIndex < 0) lastIndex = 0;
                for (var i = _doCommands.Count - 1; i >= lastIndex; i--)
                    undoList += "\n" + _doCommands[i].Description;

                return undoList;
            }

            return null;
        }

        //текстовый список команд, которые можно вернуть (не более count, -1 - все)
        internal string RedoStringList(int count = -1)
        {
            if ((_undoCommands.Count > 0) && (count >= 0))
            {
                string redoList = "";
                int lastIndex = _doCommands.Count - count;
                if (lastIndex < 0) lastIndex = 0;
                for (var i = _undoCommands.Count - 1; i >= lastIndex; i--)
                    redoList += "\n" + _undoCommands[i].Description;

                return redoList;
            }

            return null;
        }

        internal void AddCommand(Command command, bool allowUndo = true)
        {
            if (allowUndo)
            {
                if ((_maxCommandCount > -1) && (_doCommands.Count == _maxCommandCount)) _doCommands.RemoveAt(0);
                _doCommands.Add(command);
                _undoCommands.Clear();
            }

            command.Do();
        }

        internal void Undo()
        {
            if (_doCommands.Count > 0)
            {
                var command = _doCommands.Last();
                _doCommands.RemoveAt(_doCommands.Count - 1);
                _undoCommands.Add(command);
                command.Undo();
                //_formGraphic.SetUndoRedoStatus(CanUndo, CanRedo);
            }
        }

        internal void Redo()
        {
            if (_undoCommands.Count > 0)
            {
                var cmd = _undoCommands.Last();
                _undoCommands.RemoveAt(_undoCommands.Count - 1);
                _doCommands.Add(cmd);
                //_formGraphic.SetUndoRedoStatus(CanUndo, CanRedo);
                cmd.Do();
            }
        }

        internal void Clear()
        {
            _doCommands.Clear();
            _undoCommands.Clear();
            //_formGraphic.SetUndoRedoStatus(CanUndo, CanRedo);
        }
    }
}
