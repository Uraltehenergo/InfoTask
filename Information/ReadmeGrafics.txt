Private Sub Eiiiea0_Click()

'Создаем экземпляр класса GraphicForm
Dim GF
Set GF = CreateObject("graphiclibrary.graphicform")

'Создаем экземпляр класса GraphicParam
Dim Param
Set Param = CreateObject("graphiclibrary.graphicparam")

'Далее заполняем свойства Параметра.
'Я делал вручную, а вообще LoadParams(string stSql)
Param.Project = "Project1"
Param.Code = "Code1"
Param.DataType = "real"
Param.Units = "Units1"
Param.Min = 0
Param.Max = 10
Param.Tag = "Tag1"
Param.Comment = "Comment1"
Param.Id = 1
Param.Name = "Name1"
Param.SubName = "SubName1"

'Являем миру окно формы
GF.Show
'привязываем базу данных к форме
Call GF.setdatabase("Access", "D:\ArhAnalyzer5\ArhAnalyzer.accdb")
'Привязываем наш Параметр к форме
GF.Addparam (Param)
'Заполняем его
GF.loadvalues ("SELECT SetNum as Id, Time, Value1 as Val, Nd1 as Nd FROM KrestVed")
'Метод Draw вызывается только раз для отрисовки оси Y и прочего искусства
GF.Draw
'Метод репайнт может вызываться много раз. Пока что он перерисовывает (рисует, если в первый раз) Параметр
GF.Repaint (Param)

'Сборка мусора
GF.Gethering

End Sub