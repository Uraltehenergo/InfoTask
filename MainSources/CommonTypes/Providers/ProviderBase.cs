using System.Collections.Generic;
using System.Threading;
using BaseLibrary;

namespace CommonTypes
{
    //Базовый класс для всех провайдеров
    public abstract class ProviderBase
    {
        protected ProviderBase() { }
        protected ProviderBase(string name, Logger logger)
        {
            Logger = logger;
            Name = name;
        }

        //Ссылка на потоок
        public Logger Logger { get; set; }
        //Имя провайдера
        public string Name { get; set; }
        //Тип провайдера
        public abstract ProviderType Type { get; }
        //Код провайдера 
        public abstract string Code { get; }
        //Свойства провайдера
        protected string ProviderInf { get; set; }
        //Кэш для идентификации соединения
        public string Hash { get; protected set; }
        //Полное описание настроек провайдера для истории
        public string Context { get { return "Провайдер " + Name + ", " + Code + "; " + Hash; } }
        //Тип настройки
        public ProviderSetupType SetupType { get; protected set; }

        //True, пока идет настройка
        public bool IsSetup { get; set; }

        //Вызов окна настройки
        public string Setup()
        {
            if (MenuCommands == null)
            {
                MenuCommands = new DicS<Dictionary<string, IMenuCommand>>();
                AddMenuCommands();
            }
            IsSetup = true;
            new ProviderSetupForm {SetupType = SetupType, Provider = (IProvider) this}.ShowDialog();
            while (IsSetup) Thread.Sleep(500);
            return ProviderInf;
        }

        //Словарь комманд открытия дилогов, ключи - имена свойств, вторые ключи - названия пунктов меню
        public DicS<Dictionary<string, IMenuCommand>> MenuCommands { get; private set; }

        //Задание комманд, вызываемых из меню
        protected abstract void AddMenuCommands();

        //Возвращет сообщение, если значение свойства prop не является целым, или не лежит в диапазоне от min до max
        //props - словарь свойств, names - словарь имен свойств
        protected string PropIsInt(string prop, Dictionary<string, string> props, Dictionary<string, string> names, int min, int max)
        {
            if (!props.ContainsKey(prop) || !names.ContainsKey(prop) || props[prop].IsEmpty()) return "";
            string err = "Значение свойства '" + names[prop] + "' должно быть целым числом";
            int res;
            if (!int.TryParse(props[prop], out res)) return err + ", а задано равным '" + (props[prop] ?? "") + "'" + Different.NewLine;
            if (res < min || res > max) return err + " в диапазоне от " + min + " до " + max + Different.NewLine;
            return "";
        }
    }
}
