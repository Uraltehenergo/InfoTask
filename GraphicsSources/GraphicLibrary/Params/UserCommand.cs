using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GraphicLibrary.Params
{
    internal enum EUserCommand
    {
        //действия с графиками //{Graphic}
        GraphicAdd,
        GraphicDelete,     //неотменяемая (?)
        GraphicShow,
        //s GraphicsShow,
        GraphicHide,
        //s GraphicsHide,
        GraphicSelect,
        GraphicChangeAxY, //{Graphic, GroupY}
        //s GraphicsChangeAxY,
        AxYJunction,   //GraphicsChangeAxis(List<Graphic>, AxY)
        //AxYSeparation, //GraphicChangeAxis(Graphic, null)
        
        //действия с осями //{GroupY}
        AxYShow,
        AxYHide,
        AxYChangeSizePos,
        //s AllAxYChangeSizePos,

        //Ось Y (интервал)  //{GroupY, Min, Max, InPercent}
        AxYSetViewY,
        //s AxYsSetViewY,

        //ось времени //{Begin, End, IntervalType}            
        SetTimeView,

        //TimeBegin, TimeEnd //{Begin, End}
        SetTimeBegiEnd,

        //Визир {Time}
        SetVizir,

        //Линия {Width}
        SetLineWidth,
        //s SetLinesWidth,

        //Несколько команд
        GroupOperation
    }

    internal class UserCommand
    {
        internal static FormGraphic Fg;

    #region Prop
        public EUserCommand Command { get; private set; }
        
        public object Object { get; private set; }
        
        private readonly List<object> _params = new List<object>();
        public List<object> Params { get { return _params; } }

        private readonly List<object> _changedParams = new List<object>();
        public List<object> ChangedParams { get { return _changedParams; } }

        public bool CanUndo 
        { 
            get
            {
                switch (Command)
                {
                    //case EUserCommand.GraphicAdd:
                    case EUserCommand.GraphicDelete:
                    case EUserCommand.SetTimeBegiEnd:
                        return false;
                }

                return true;
            }
        }
        
        public string Description
        {
            get
            {
                switch(Command)
                {
                    case EUserCommand.GraphicShow:
                        return "Показать график";
                    case EUserCommand.GraphicHide:
                        return "Скрыть график";
                    //case EUserCommand.GraphicSelect
                    case EUserCommand.GraphicChangeAxY:
                        return "Изменить ось графика";
                    //действия с осями //{GroupY}
                    case EUserCommand.AxYJunction:
                        return "Объединить оси";
                    case EUserCommand.AxYShow:
                        return "Показать ось";
                    case EUserCommand.AxYHide:
                        return "Скрыть ось";
                    case EUserCommand.AxYChangeSizePos:
                        return "Изменить размер/положение оси";
                    //Ось Y (интервал)
                    case EUserCommand.AxYSetViewY:
                        return "Изменить шкалу оси";
                    //ось времени
                    case EUserCommand.SetTimeView:
                        return "Изменить шкалу времени";
                    //Визир
                    case EUserCommand.SetVizir:
                        return "Установить визир";
                    //Линия {Width}    
                    case EUserCommand.SetLineWidth:
                        return "Установить толщину линии";
                    //Несколько команд
                    case EUserCommand.GroupOperation:
                        switch (((UserCommand)_params.First()).Command)
                        {
                            case EUserCommand.GraphicShow:
                                return "Показать графики";
                            case EUserCommand.GraphicHide:
                                return "Скрыть графики";
                            //case EUserCommand.GraphicSelect
                            case EUserCommand.GraphicChangeAxY:
                                return "Изменить ось графиков";
                            //действия с осями //{GroupY}
                            case EUserCommand.AxYJunction:
                                return "Объединить оси";
                            case EUserCommand.AxYShow:
                                return "Показать оси";
                            case EUserCommand.AxYHide:
                                return "Скрыть оси";
                            case EUserCommand.AxYChangeSizePos:
                                return "Изменить размер/положение осей";
                            //Ось Y (интервал)
                            case EUserCommand.AxYSetViewY:
                                return "Изменить шкалу осей";
                            //ось времени
                            case EUserCommand.SetTimeView:
                                return "Изменить шкалу времени";
                            //Визир
                            case EUserCommand.SetVizir:
                                return "Установить визир";
                            //Линия {Width}    
                            case EUserCommand.SetLineWidth:
                                return "Установить толщину линий";
                        }
                        break;
                }
                
                if (Command == EUserCommand.GroupOperation) return "<" + ((UserCommand) _params.First()).Description + ">";
                return Command.ToString();
            }
        }
    #endregion Prop

    #region Init
        public UserCommand(EUserCommand command, object obj, List<object> paramz = null, List<object> changedParamz = null)
        {
            Command = command;
            Object = obj;

            _params = paramz;
            _changedParams = changedParamz;
        }

        public UserCommand(EUserCommand command, object obj, object param = null, object changedParam = null)
        {
            Command = command;
            Object = obj;

            if (param != null) _params.Add(param);
            if (changedParam != null) _changedParams.Add(changedParam);
        }

        public UserCommand(EUserCommand command, object obj, object param = null, List<object> changedParamz = null)
        {
            Command = command;
            Object = obj;

            if (param != null) _params.Add(param);
            _changedParams = changedParamz;
        }

        public UserCommand(EUserCommand command, object obj)
        {
            Command = command;
            Object = obj;

            //_params.Clear();
            //_changedParams.Clear();
        }
    #endregion Init

    #region Public
        //changeParam - параметр для операция, которые возможно выполняются по факту
        //например при установке визира нажатием точки
        public bool Do(bool changeParam = true)
        {
            bool res = true;

            try
            {
                Graphic gr;
                //s List<Graphic> grs;
                GroupY axY;
                //s List<GroupY> axYs;
                //s int i;

                switch (Command)
                {
                    //case EUserCommand.GraphicAdd:
                    //    gr = (Graphic) Object;
                    //    //if (gr.IsAnalog)
                    //    //    _fg.AddAnalogGraphic(gr);
                    //    //else
                    //    //    _fg.AddDiscretGraphic(gr);
                    //    break;

                    //case EUserCommand.GraphicDelete:
                    //    gr = (Graphic) Object;
                    //    Fg.DeleteGraphic(gr);
                    //    break;

                    case EUserCommand.GraphicShow:
                        gr = (Graphic) Object;
                        Fg.CmdGraphicShow(gr);
                        break;

                    //s case EUserCommand.GraphicsShow:
                    //s     grs = (List<Graphic>) Object;
                    //s     Fg.CmdGraphicShow(grs);
                    //s     break;

                    case EUserCommand.GraphicHide:
                        gr = (Graphic) Object;
                        Fg.CmdGraphicHide(gr);
                        break;

                    //s case EUserCommand.GraphicsHide:
                    //s     grs = (List<Graphic>) Object;
                    //s     Fg.CmdGraphicHide(grs);
                    //s     break;

                    //case EUserCommand.GraphicSelect:
                    //    gr = (Graphic) Object;
                    //    //_fg.SelectGraphic(gr);
                    //    Fg.CurGraphicNum = gr.Num; //заменить на SelectGraphic
                    //    break;

                    case EUserCommand.GraphicChangeAxY:
                        var aGr = (AnalogGraphic) Object;
                        if (_params.Count > 0) axY = (GroupY) _params[0]; else axY = null;
                        Fg.CmdGraphicChangeAxY(aGr, axY);
                        break;

                    case EUserCommand.AxYShow:
                        axY = (GroupY) Object;
                        Fg.CmdAxYShow(axY);
                        break;

                    case EUserCommand.AxYHide:
                        axY = (GroupY) Object;
                        Fg.CmdAxYHide(axY);
                        break;

                    case EUserCommand.AxYChangeSizePos:
                        axY = (GroupY) Object;
                        var axSize = (double) _params[0];
                        var axPos = (double) _params[1];
                        Fg.CmdAxYChangeSizePos(axY, axSize, axPos); 
                        break;

                    //case EUserCommand.AxYJunction:
                    //    var catcher = ((AnalogGraphic) Object).GroupY;
                    //    var pitcher = ((AnalogGraphic) _params[0]).GroupY;
                    //    Fg.CmdAxYJunction(catcher, pitcher);
                    //    break;

                    case EUserCommand.AxYJunction:
                        var catcher = (GroupY) Object;
                        var pitcher = (GroupY) _params[0];
                        Fg.CmdAxYJunction(catcher, pitcher);
                        break;

                    //case EUserCommand.AllAxisChangeSizePos:
                    //    axYs = (List<GroupY>) Object;
                    //    var axSizes = (List<double>) _params[0];
                    //    var axPoses = (List<double>) _params[1];
                    //    i = -1;
                    //    foreach (var aY in axYs)
                    //    {
                    //        i++;
                    //        //_fg.AxResize(aY, axSizes[i], axPoses[i]);
                    //    }
                    //    break;

                    case EUserCommand.AxYSetViewY:
                        axY = (GroupY) Object;
                        var minView = (double) _params[0];
                        var maxView = (double) _params[1];
                        var isPrcnt = (bool) _params[2];
                        Fg.CmdAxYSetViewY(axY, minView, maxView, isPrcnt);
                        break;

                    //s case EUserCommand.AxYsSetViewY:
                    //s     axYs = (List<GroupY>) Object;
                    //s     var minVs = (List<double>) _params[0];
                    //s     var maxVs = (List<double>) _params[1];
                    //s     var isPs = (List<bool>) _params[2];
                    //s     Fg.CmdAxYSetViewY(axYs, minVs, maxVs, isPs);
                    //s     break;

                    case EUserCommand.SetTimeView:
                        if(changeParam)
                        {
                            var viewTimeBeg = (DateTime) _params[0];
                            var viewTimeEnd = (DateTime) _params[1];
                            var scaleType = (DateTimeIntervalType) _params[2];
                            Fg.CmdSetTimeView(viewTimeBeg, viewTimeEnd, scaleType);
                        }
                        break;

                    //case EUserCommand.SetTimeBegiEnd:
                    //    var timeBegin = (DateTime) _params[0];
                    //    var timeEnd = (DateTime) _params[1];
                    //    //_fg.SetTimeBegiEnd(timeBegin, timeEnd);
                    //    //_fg.TimeBegin = timeBegin;  //заменить на SetTimeBegiEnd
                    //    //_fg.TimeEnd = timeEnd;      //заменить на SetTimeBegiEnd
                    //    break;

                    case EUserCommand.SetVizir:
                        var vizirTime = (DateTime) _params[0];
                        if (vizirTime != DateTime.MinValue)
                            Fg.CmdSetVizir(vizirTime, changeParam);
                        else
                            Fg.CmdUnsetVizir();
                        break;

                    case EUserCommand.SetLineWidth:
                        gr = (Graphic) Object;
                        var lineWidth = (int) _params[0];
                        Fg.CmdSetLineWidth(gr, lineWidth);
                        break;

                    //s case EUserCommand.SetLinesWidth:
                    //s     grs = (List<Graphic>) Object;
                    //s     lineWidth = (int) _params[0];
                    //s     Fg.CmdSetLineWidth(grs, lineWidth);
                    //s     break;

                    case EUserCommand.GroupOperation:
                        foreach (var cmd in _params)
                            ((UserCommand) cmd).Do();
                        break;
                }
            }
            catch(Exception e)
            {
                res = false;
                MessageBox.Show("UserCommand.Do\n" + e.Message + "\n" + e.StackTrace);
            }
            
            return res;
        }

        public bool Undo()
        {
            bool res = true;

            try
            {
                Graphic gr;
                AnalogGraphic aGr;
                //s List<Graphic> grs;
                GroupY axY;
                //s List<GroupY> axYs;
                DateTime viewTimeBeg;
                DateTime viewTimeEnd;
                DateTimeIntervalType scaleType;
                double axSize;
                double axPos;
                double viewMin;
                double viewMax;
                bool isPrcnt;
                //s int i;

                switch (Command)
                {
                    //case EUserCommand.GraphicAdd:
                    //    gr = (Graphic) Object;
                    //    Fg.DeleteGraphic(gr);
                    //    break;

                    //case EUserCommand.GraphicDelete: //неотменяемая
                    //    break;

                    case EUserCommand.GraphicShow:
                        gr = (Graphic) Object;
                        Fg.CmdGraphicHide(gr);
                        break;

                    //s case EUserCommand.GraphicsShow:
                    //s     grs = (List<Graphic>) Object;
                    //s     Fg.CmdGraphicHide(grs);
                    //s     break;

                    case EUserCommand.GraphicHide:
                        gr = (Graphic) Object;
                        Fg.CmdGraphicShow(gr);
                        break;

                    //s case EUserCommand.GraphicsHide:
                    //s     grs = (List<Graphic>) Object;
                    //s     Fg.CmdGraphicShow(grs);
                    //s     break;

                    //case EUserCommand.GraphicSelect:
                    //    var grNum = (int) _changedParams[0];
                    //    Fg.CurGraphicNum = grNum; //заменить на SelectGraphic
                    //    break;

                    case EUserCommand.GraphicChangeAxY:
                        aGr = (AnalogGraphic) Object;
                        axY = (GroupY) _changedParams[0];
                        Fg.CmdGraphicChangeAxY(aGr, axY);
                        break;

                    case EUserCommand.AxYShow:
                        axY = (GroupY) Object;
                        Fg.CmdAxYHide(axY);
                        break;

                    case EUserCommand.AxYHide:
                        axY = (GroupY) Object;
                        Fg.CmdAxYShow(axY);
                        break;

                    case EUserCommand.AxYChangeSizePos:
                        axY = (GroupY) Object;
                        axSize = (double) _changedParams[0];
                        axPos = (double) _changedParams[1];
                        Fg.CmdAxYChangeSizePos(axY, axSize, axPos);
                        break;

                    //case EUserCommand.AxYJunction:
                    //    var cUpGr = (AnalogGraphic) Object;
                    //    var pUpGr = (AnalogGraphic) _params[0];
                    //    var grs = (List<AnalogGraphic>) _changedParams[0];
                    //    viewMin = (double) _changedParams[1];
                    //    viewMax = (double) _changedParams[2];
                    //    isPrcnt = (bool) _changedParams[3];
                    //    axSize = (double) _changedParams[4];
                    //    axPos = (double) _changedParams[5];
                    //    Fg.CmdAxYJunctionRepeal(grs, viewMin, viewMax, isPrcnt, axSize, axPos, pUpGr, cUpGr);
                    //    break;

                    case EUserCommand.AxYJunction:
                        var catcher = (GroupY) Object;
                        var pitcher = (GroupY) _params[0];
                        var grs = (List<AnalogGraphic>) _changedParams[0];
                        aGr = (AnalogGraphic) _changedParams[1];
                        Fg.CmdAxYJunctionRepeal(pitcher, grs, aGr);
                        break;

                    //case EUserCommand.AllAxisChangeSizePos:
                    //    axYs = (List<GroupY>) Object;
                    //    var axSizes = (List<double>) _changedParams[0];
                    //    var axPoses = (List<double>) _changedParams[1];
                    //    i = -1;
                    //    foreach (var aY in axYs)
                    //    {
                    //        i++;
                    //        //_fg.AxResize(aY, axSizes[i], axPoses[i]);
                    //    }
                    //    break;

                    case EUserCommand.AxYSetViewY:
                        axY = (GroupY) Object;
                        viewMin = (double) _changedParams[0];
                        viewMax = (double) _changedParams[1];
                        isPrcnt = (bool) _changedParams[2];
                        Fg.CmdAxYSetViewY(axY, viewMin, viewMax, isPrcnt);
                        break;

                    //s case EUserCommand.AxYsSetViewY:
                    //s     axYs = (List<GroupY>) Object;
                    //s     var minVs = (List<double>) _changedParams[0];
                    //s     var maxVs = (List<double>) _changedParams[1];
                    //s     var isPs = (List<bool>) _changedParams[2];
                    //s     Fg.CmdAxYSetViewY(axYs, minVs, maxVs, isPs);
                    //s     break;

                    case EUserCommand.SetTimeView:
                        viewTimeBeg = (DateTime) _changedParams[0];
                        viewTimeEnd = (DateTime) _changedParams[1];
                        scaleType = (DateTimeIntervalType) _changedParams[2];
                        Fg.CmdSetTimeView(viewTimeBeg, viewTimeEnd, scaleType);
                        break;

                    //case EUserCommand.SetTimeBegiEnd: //неотменяемая
                    //    break;

                    case EUserCommand.SetVizir:
                        var vizirTime = (DateTime) _changedParams[0];
                        viewTimeBeg = (DateTime)_changedParams[1];
                        viewTimeEnd = (DateTime)_changedParams[2];
                        scaleType = (DateTimeIntervalType) _changedParams[3];

                        if (vizirTime != DateTime.MinValue) 
                            Fg.CmdSetVizir(vizirTime);
                        else 
                            Fg.CmdUnsetVizir();

                        //возвращаем отображаемый период
                        Fg.CmdSetTimeView(viewTimeBeg, viewTimeEnd, scaleType);
                        break;

                    case EUserCommand.SetLineWidth:
                        gr = (Graphic) Object;
                        var lineWidth = (int) _changedParams[0];
                        Fg.CmdSetLineWidth(gr, lineWidth);
                        break;

                    //s case EUserCommand.SetLinesWidth:
                    //s     grs = (List<Graphic>) Object;
                    //s     var linesWidth = (List<int>) _changedParams[0];
                    //s     i = -1;
                    //s     foreach(var grap in grs)
                    //s     {
                    //s         i++;
                    //s         Fg.CmdSetLineWidth(grap, linesWidth[i]);
                    //s     }
                    //s     break;

                    case EUserCommand.GroupOperation:
                        //foreach (var cmd in _params)
                        //    ((UserCommand)cmd).Undo();
                        for (int i = _params.Count - 1; i >= 0; i--)
                            ((UserCommand)_params[i]).Undo();
                        break;
                }
            }
            catch (Exception e)
            {
                res = false;
                MessageBox.Show("UserCommand.UnDo\n" + e.Message + "\n" + e.StackTrace);
            }

            return res;
        }
    #endregion Public
    }

    internal class UserCommandList
    {
        private readonly FormGraphic _formGraphic;
         
        private readonly int _maxCommandCount = 0; //Максимальное количество команд в списке
                                                   //Если 0, неограничено

        public UserCommandList(FormGraphic formGraphic, int maxCommandCount = 0)
        {
            _formGraphic = formGraphic;
            UserCommand.Fg = formGraphic;
            _maxCommandCount = maxCommandCount;
        }

        private readonly List<UserCommand> _commands = new List<UserCommand>();
        private readonly List<UserCommand> _undoCommands = new List<UserCommand>();
        
        internal bool CanUndo
        {
            get { return (_commands.Count > 0); }
        }

        internal bool CanRedo
        {
            get { return (_undoCommands.Count > 0); }
        }

        //текстовый список команд, которые можно отменить (не более count последних, 0 - все)
        internal string UndoList(int count = 0)
        {
            if (_commands.Count > 0)
            {
                string undoList = "";
                int j = 0;
                for (var i = _commands.Count - 1; i >= 0; i--)
                {
                    undoList += "\n" + _commands[i].Description;
                    j++;
                    if ((count > 0) && (j == count)) break;
                }

                return undoList;
            }

            return null;
        }

        //текстовый список команд, которые можно вернуть (не более count, 0 - все)
        internal string RedoList(int count = 0)
        {
            if (_undoCommands.Count > 0)
            {
                string redoList = "";
                int j = 0;
                for (var i = _undoCommands.Count - 1; i >= 0; i--)
                {
                    j++;
                    redoList += "\n" + _undoCommands[i].Description;
                    if ((count > 0) && (j == count)) break;
                }

                return redoList;
            }

            return null;
        }
        
        private void AddCommand(UserCommand command, bool changeParam = true)
        {
            if(command.CanUndo)
            {
                if (!_continueGroup)
                {
                    if ((_maxCommandCount > 0) && (_commands.Count == _maxCommandCount)) _commands.RemoveAt(0);
                    _commands.Add(command);
                    _undoCommands.Clear();

                    _formGraphic.SetUndoRedoStatus(CanUndo, CanRedo);
                }
                else
                {
                    var cmd = _commands.Last();
                    if (cmd.Command != EUserCommand.GroupOperation)
                    {
                        _commands.RemoveAt(_commands.Count - 1);
                        var grCmd = new UserCommand(EUserCommand.GroupOperation, null, cmd, null);
                        grCmd.Params.Add(command);
                        _commands.Add(grCmd);
                    }
                    else
                        cmd.Params.Add(command);
                }
            }

            command.Do(changeParam);
            
            _continueGroup = false;
        }

        internal void Undo()
        {
            if(_commands.Count > 0)
            {
                var cmd = _commands.Last();
                bool res = cmd.Undo();
                if (res)
                {
                    _commands.RemoveAt(_commands.Count - 1);
                    _undoCommands.Add(cmd);
                    _formGraphic.SetUndoRedoStatus(CanUndo, CanRedo);
                }
            }
        }

        internal void Redo()
        {
            if (_undoCommands.Count > 0)
            {
                var cmd = _undoCommands.Last();
                
                bool res = cmd.Do();
                if (res)
                {
                    _undoCommands.RemoveAt(_undoCommands.Count - 1);
                    _commands.Add(cmd);
                    _formGraphic.SetUndoRedoStatus(CanUndo, CanRedo);
                }
            }
        }

        private bool _continueGroup = false;
        internal void ContinueGroup()
        {
            _continueGroup = true;
        }

        internal void Clear()
        {
            _commands.Clear();
            _undoCommands.Clear();
            _formGraphic.SetUndoRedoStatus(CanUndo, CanRedo);
        }

    #region Function
        //internal bool GraphicAdd(Graphic graphic)
        //{
        //    if (graphic != null)
        //    {
        //        var cmd = new UserCommand(EUserCommand.GraphicAdd, graphic, null);
        //        AddCommand(cmd);
        //        return true;
        //    }

        //    return false;
        //}
        
        //internal bool GraphicDelete(Graphic graphic){}
        
        internal bool GraphicShow(Graphic graphic)
        {
            if (!graphic.IsVisible)
            {
                var cmd = new UserCommand(EUserCommand.GraphicShow, graphic, null);
                AddCommand(cmd);
                return true;
            }

            return false;
        }

        /*internal bool GraphicShow(List<Graphic> graphics)
        {
            bool fg = false;
            foreach (var gr in graphics)
            {
                if (!gr.IsVisible)
                {
                    fg = true;
                    break;
                }
            }

            if (fg)
            {
                var cmd = new UserCommand(EUserCommand.GraphicsShow, graphics, null);
                AddCommand(cmd);
                return true;
            }

            return false;
        }*/
        
        internal bool GraphicHide(Graphic graphic)
        {
            if (graphic.IsVisible)
            {
                var cmd = new UserCommand(EUserCommand.GraphicHide, graphic, null);
                AddCommand(cmd);
                return true;
            }

            return false;
        }

        /*internal bool GraphicHide(List<Graphic> graphics)
        {
            bool fg = false;
            foreach (var gr in graphics)
            {
                if (gr.IsVisible)
                {
                    fg = true;
                    break;
                }
            }

            if (fg)
            {
                var cmd = new UserCommand(EUserCommand.GraphicsHide, graphics, null);
                AddCommand(cmd);
                return true;
            }

            return false;
        }*/
        
        //internal bool GraphicSelect(Graphic graphic){}

        //axY = null, если в новую ось
        internal bool GraphicChangeAxY(AnalogGraphic graphic, GroupY axY)
        {
            if ((graphic.GroupY != axY) && ((graphic.GroupY.IsOverlayed) || (axY != null)))
            {
                var cmd = new UserCommand(EUserCommand.GraphicChangeAxY, graphic, axY, graphic.GroupY);
                AddCommand(cmd);
                return true;
            }

            return false;
        }
        
        internal bool AxYShow(GroupY axY)
        {
            if (!axY.IsVisible)
            {
                var cmd = new UserCommand(EUserCommand.AxYShow, axY, null);
                AddCommand(cmd);
                return true;
            }

            return false;
        }

        internal bool AxYHide(GroupY axY)
        {
            if (axY.IsVisible)
            {
                var cmd = new UserCommand(EUserCommand.AxYHide, axY, null);
                AddCommand(cmd);
                return true;
            }

            return false;
        }

        internal bool AxYChangeSizePos(GroupY axY, double newSize, double newPos)
        {
            if((axY.CurAxSize != newSize) || (axY.CurAxPos != newPos))
            {
                var pz = new List<object> { newSize, newPos };
                var cpz = new List<object> { axY.CurAxSize, axY.CurAxPos };
                var cmd = new UserCommand(EUserCommand.AxYChangeSizePos, axY, pz, cpz);
                AddCommand(cmd);
                return true;
            }

            return false;
        }

        internal bool AxYSizePosChanged(GroupY axY, double curSize, double curPos)
        {
            if ((axY.CurAxSize != curSize) || (axY.CurAxPos != curPos))
            {
                var pz = new List<object> { axY.CurAxSize, axY.CurAxPos };
                var cpz = new List<object> { curSize, curPos };
                var cmd = new UserCommand(EUserCommand.AxYChangeSizePos, axY, pz, cpz);
                AddCommand(cmd, false);
                return true;
            }

            return false;
        }

        //internal bool AxYJunction(GroupY axY, GroupY axCatcher)
        //{
        //    var pz = new List<object> { axCatcher };
        //    var cpz = new List<object> { curViewTimeBegin, curViewTimeEnd, curScaleType };

        //    var cmd = new UserCommand(EUserCommand.AxYJunction, graphic, axY, graphic.GroupY);
        //}
        
        //internal bool AxYChangeSizePos(GroupY axY, double newSize, double newPos){}

        //internal bool AllAxYChangeSizePos(List<GroupY> axY, List<double> newSizes, List<double> newPoses){}

        internal bool AxYSetViewY(GroupY axY, double viewYMin, double viewYMax, bool inPercent)
        {
            if ((viewYMin != axY.ViewMin) || (viewYMax != axY.ViewMax) || (inPercent != axY.IsInPercent))
            {
                var pz = new List<object> { viewYMin, viewYMax, inPercent };
                var cpz = new List<object> { axY.ViewMin, axY.ViewMax, axY.IsInPercent };
                var cmd = new UserCommand(EUserCommand.AxYSetViewY, axY, pz, cpz);
                //var cmd = new UserCommand(EUserCommand.AxYSetViewY, axY.UpperGraphic, pz, cpz);
                AddCommand(cmd);
                return true;
            }
            return false;
        }

        /*internal bool AxYSetViewY(List<GroupY> axYs, List<double> viewYMins, List<double> viewYMaxs, List<bool> inPercents)
        {
            var cViewYMin = new List<double>();
            var cViewYMax = new List<double>();
            var cInPercents = new List<bool>();
            bool fg = false;
            int i = -1;
            foreach (var axY in axYs)
            {
                i++;
                cViewYMin.Add(axY.ViewMin);
                cViewYMax.Add(axY.ViewMax);
                cInPercents.Add(axY.IsInPercent);

                if ((viewYMins[i] != axY.ViewMin) || (viewYMaxs[i] != axY.ViewMax) || (inPercents[i] != axY.IsInPercent))
                    fg = true;
            }

            if (fg)
            {
                var pz = new List<object> { viewYMins, viewYMaxs, inPercents };
                var cpz = new List<object> { cViewYMin, cViewYMax, cInPercents };
                var cmd = new UserCommand(EUserCommand.AxYsSetViewY, axYs, pz, cpz);
                AddCommand(cmd);
                return true;
            }

            return false;
        }*/

        //internal bool AxYJunction(GroupY catcher, GroupY pitcher)
        //{
        //    if (catcher != pitcher)
        //    {
        //        var cpz = new List<Object>();

        //        var grs = new List<AnalogGraphic>();
        //        foreach (var gr in pitcher.Graphics) grs.Add(gr);
        //        cpz.Add(grs);
        //        cpz.Add(pitcher.ViewMin);
        //        cpz.Add(pitcher.ViewMax);
        //        cpz.Add(pitcher.IsInPercent);
        //        cpz.Add(pitcher.CurAxSize);
        //        cpz.Add(pitcher.CurAxPos);

        //        var cmd = new UserCommand(EUserCommand.AxYJunction, catcher.UpperGraphic, pitcher.UpperGraphic, cpz);
        //        AddCommand(cmd);
        //        return true;
        //    }

        //    return false;
        //}

        internal bool AxYJunction(GroupY catcher, GroupY pitcher)
        {
            if (catcher != pitcher)
            {
                var cpz = new List<Object>();
                var grs = new List<AnalogGraphic>();
                foreach (var gr in pitcher.Graphics) grs.Add(gr);
                cpz.Add(grs);
                cpz.Add(catcher.UpperGraphic);
                cpz.Add(pitcher.UpperGraphic);

                var cmd = new UserCommand(EUserCommand.AxYJunction, catcher, pitcher, cpz);
                AddCommand(cmd);
                return true;
            }

            return false;
        }
        
        //Автоопределение _axisXIntervalType
        internal bool SetTimeView(DateTime viewTimeBegin, DateTime viewTimeEnd,
                                  DateTime curViewTimeBegin, DateTime curViewTimeEnd, DateTimeIntervalType curScaleType)
        {
            if ((viewTimeBegin != curViewTimeBegin) || (viewTimeEnd != curViewTimeEnd))
            {
                var pz = new List<object> { viewTimeBegin, viewTimeEnd, DateTimeIntervalType.NotSet };
                var cpz = new List<object> { curViewTimeBegin, curViewTimeEnd, curScaleType };
                var cmd = new UserCommand(EUserCommand.SetTimeView, null, pz, cpz);
                AddCommand(cmd);
                return true;
            }

            return false;
        }

        //Если viewTimeIntervalType = DateTimeIntervalType.NotSet - автоопределение _axisXIntervalType
        internal bool SetTimeView(DateTime viewTimeBegin, DateTime viewTimeEnd, DateTimeIntervalType scaleType,
                                  DateTime curViewTimeBegin, DateTime curViewTimeEnd, DateTimeIntervalType curScaleType)
        {
            if ((viewTimeBegin != curViewTimeBegin) || (viewTimeEnd != curViewTimeEnd) || (scaleType != curScaleType))
            {
                var pz = new List<object> { viewTimeBegin, viewTimeEnd, scaleType };
                var cpz = new List<object> { curViewTimeBegin, curViewTimeEnd, curScaleType };
                var cmd = new UserCommand(EUserCommand.SetTimeView, null, pz, cpz);
                AddCommand(cmd);
                return true;
            }

            return false;
        }

        internal void HScrollerScroll(DateTime viewTimeBegin, DateTime viewTimeEnd, DateTimeIntervalType scaleType,
                                      DateTime curViewTimeBegin, DateTime curViewTimeEnd)
        {
            var pz = new List<object> { viewTimeBegin, viewTimeEnd, scaleType };
            var cpz = new List<object> { curViewTimeBegin, curViewTimeEnd, scaleType };
            var cmd = new UserCommand(EUserCommand.SetTimeView, null, pz, cpz);
            AddCommand(cmd, false);
        }

        //internal bool SetTimeBegiEnd(DateTime timeBegin, DateTime timeEnd){}

        //если визир не установлен, используется DateTime.MinValue
        //поскольку установка визира может менять область отображения, для отмены необходимы времена начала и конца отображаемого периода
        internal bool SetVizir(DateTime vizirTime, DateTime curVizirTime, DateTime curViewMin, DateTime curViewMax,
                               DateTimeIntervalType curViewTimeIntervalType)
        {
            if (curVizirTime != vizirTime)
            {
                var cpz = new List<object> { curVizirTime, curViewMin, curViewMax, curViewTimeIntervalType };
                var cmd = new UserCommand(EUserCommand.SetVizir, null, vizirTime, cpz);
                AddCommand(cmd);
                return true;
            }

            return false;
        }

        internal bool VizirChanged(DateTime vizirTime, DateTime curVizirTime, DateTime curViewMin, DateTime curViewMax, 
                                   DateTimeIntervalType curViewTimeIntervalType)
        {
            if (curVizirTime != vizirTime)
            {
                var cpz = new List<object> { curVizirTime, curViewMin, curViewMax, curViewTimeIntervalType };
                var cmd = new UserCommand(EUserCommand.SetVizir, null, vizirTime, cpz);
                AddCommand(cmd, false);
                return true;
            }

            return false;
        }

        internal bool SetLineWidth(Graphic graphic, int lineWidth)
        {
            if ((graphic != null) && (graphic.LineWidth != lineWidth))
            {
                var cmd = new UserCommand(EUserCommand.SetLineWidth, graphic, lineWidth, graphic.LineWidth);
                AddCommand(cmd);
                return true;
            }

            return false;
        }

        /*internal bool SetLineWidth(List<Graphic> graphics, int lineWidth)
        {
            var cpz = new List<int>();
            bool fg = false;
            foreach (var gr in graphics)
            {
                if (gr.LineWidth != lineWidth) fg = true;
                cpz.Add(gr.LineWidth);
            }

            if (fg)
            {
                var cmd = new UserCommand(EUserCommand.SetLinesWidth, graphics, lineWidth, cpz);
                AddCommand(cmd);        
                return true;
            }

            return false;
        }*/
    #endregion Function
    }
    

    /*
    internal class UserCommandNew
    {
        internal static Action<object, object> CommandDo;
        internal static Action<object, object> CommandUndo;

    #region Prop
        public EUserCommand Command { get; private set; }

        public object Object { get; private set; }

        private readonly List<object> _params = new List<object>();
        public List<object> Params { get { return _params; } }

        private readonly List<object> _changedParams = new List<object>();
        public List<object> ChangedParams { get { return _changedParams; } }

        public string Description { get; set; }
    #endregion Prop

    #region Init
        public UserCommandNew(EUserCommand command, object obj, List<object> paramz = null, List<object> changedParamz = null)
        {
            Command = command;
            Object = obj;

            _params = paramz;
            _changedParams = changedParamz;
        }

        public UserCommandNew(EUserCommand command, object obj, object param = null, object changedParam = null)
        {
            Command = command;
            Object = obj;

            if (param != null) _params.Add(param);
            if (changedParam != null) _changedParams.Add(changedParam);
        }

        public UserCommandNew(EUserCommand command, object obj, object param = null, List<object> changedParamz = null)
        {
            Command = command;
            Object = obj;

            if (param != null) _params.Add(param);
            _changedParams = changedParamz;
        }
    #endregion Init

    #region Public
        //changeParam - параметр для операция, которые возможно выполняются по факту
        //например при установке визира нажатием точки
        public bool Do()
        {
            bool res = true;

            try
            {
                CommandDo(Object, _params);
            }
            catch (Exception e)
            {
                res = false;
                MessageBox.Show("UserCommand.Do\n" + e.Message + "\n" + e.StackTrace);
            }

            return res;
        }

        public bool Undo()
        {
            bool res = true;

            try
            {
                CommandDo(Object, _changedParams);
            }
            catch (Exception e)
            {
                res = false;
                MessageBox.Show("UserCommand.UnDo\n" + e.Message + "\n" + e.StackTrace);
            }

            return res;
        }
    #endregion Public
    }

    internal class UserCommandListNew
    {
        private readonly int _maxCommandCount = 0; //Максимальное количество команд в списке (0, если неограничено)

        public UserCommandListNew(Action<object, object> commandDo, Action<object, object> commandUndo, int maxCommandCount = 0)
        {
            UserCommandNew.CommandDo += commandDo;
            UserCommandNew.CommandUndo += commandUndo;
            _maxCommandCount = maxCommandCount;
        }

        private readonly List<UserCommandNew> _commands = new List<UserCommandNew>();
        private readonly List<UserCommandNew> _undoCommands = new List<UserCommandNew>();

        internal bool CanUndo
        {
            get { return (_commands.Count > 0); }
        }

        internal bool CanRedo
        {
            get { return (_undoCommands.Count > 0); }
        }

        //текстовый список команд, которые можно отменить (не более count последних, 0 - все)
        internal string UndoList(int count = 0)
        {
            if (_commands.Count > 0)
            {
                string undoList = "";
                int j = 0;
                for (var i = _commands.Count - 1; i >= 0; i--)
                {
                    undoList += "\n" + _commands[i].Description;
                    j++;
                    if ((count > 0) && (j == count)) break;
                }

                return undoList;
            }

            return null;
        }

        //текстовый список команд, которые можно вернуть (не более count, 0 - все)
        internal string RedoList(int count = 0)
        {
            if (_undoCommands.Count > 0)
            {
                string redoList = "";
                int j = 0;
                for (var i = _undoCommands.Count - 1; i >= 0; i--)
                {
                    j++;
                    redoList += "\n" + _undoCommands[i].Description;
                    if ((count > 0) && (j == count)) break;
                }

                return redoList;
            }

            return null;
        }

        internal void AddCommand(UserCommandNew command)
        {
            if ((_maxCommandCount > 0) && (_commands.Count == _maxCommandCount)) _commands.RemoveAt(0);
            _commands.Add(command);
            _undoCommands.Clear();
        }

        internal void Undo()
        {
            if (_commands.Count > 0)
            {
                var cmd = _commands.Last();
                bool res = cmd.Undo();
                _commands.RemoveAt(_commands.Count - 1);
                _undoCommands.Add(cmd);
            }
        }

        internal void Redo()
        {
            if (_undoCommands.Count > 0)
            {
                var cmd = _undoCommands.Last();

                bool res = cmd.Do();
                _undoCommands.RemoveAt(_undoCommands.Count - 1);
                _commands.Add(cmd);
            }
        }
    }
    */
}
