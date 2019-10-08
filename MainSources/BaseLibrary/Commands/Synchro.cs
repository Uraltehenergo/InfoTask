using System;
using System.Threading;

namespace BaseLibrary
{
    //Объект, ограничивающий количество обращений к одному провайдеру
    public class Synchro
    {
        //Количество обращений к провайдеру
        private readonly DicS<int> _providers = new DicS<int>();
        private readonly object _providersLock = new object();
        
        //Запуск нового процесса
        public void StartProcess(string process, //Имя процесса
                                  int maxCount = 1) //Максимальное количество параллельных запусков процесса
        {
            StartProcess(process, new Tuple<string, int>(process, maxCount));
        }

        public void StartProcess(string process, //Имя процесса
                                  int maxCount, //Максимальное количество параллельных запусков процесса
                                  string process2, int maxCount2 = 1)
        {
            StartProcess(process, new Tuple<string, int>(process, maxCount), 
                                  new Tuple<string, int>(process2, maxCount2));
        }
        
        //Запуск нового процесса
        public void StartProcess(string process, //Имя процесса
                                    params Tuple<string, int>[] bounds) //Ограничения на количества процессов, которые должны быть соблюдены
        {
            while (true)
            {
                lock (_providersLock)
                {
                    bool b = false;
                    foreach (var bound in bounds)
                        b |= _providers.Get(bound.Item1, 0) >= bound.Item2;
                    if (!b) break;
                }
                Thread.Sleep(100);
            }
            lock (_providersLock)
            {
                if (!_providers.ContainsKey(process))
                    _providers.Add(process, 1);
                else _providers[process]++;
            }
        }

        //Завершен процесс с указанным именем
        public void FinishProcess(string process)
        {
            lock (_providersLock)
                if (_providers.Get(process, 0) > 0)
                    _providers[process]--;
        }
    }
}