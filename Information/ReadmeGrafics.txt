Private Sub Eiiiea0_Click()

'������� ��������� ������ GraphicForm
Dim GF
Set GF = CreateObject("graphiclibrary.graphicform")

'������� ��������� ������ GraphicParam
Dim Param
Set Param = CreateObject("graphiclibrary.graphicparam")

'����� ��������� �������� ���������.
'� ����� �������, � ������ LoadParams(string stSql)
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

'������ ���� ���� �����
GF.Show
'����������� ���� ������ � �����
Call GF.setdatabase("Access", "D:\ArhAnalyzer5\ArhAnalyzer.accdb")
'����������� ��� �������� � �����
GF.Addparam (Param)
'��������� ���
GF.loadvalues ("SELECT SetNum as Id, Time, Value1 as Val, Nd1 as Nd FROM KrestVed")
'����� Draw ���������� ������ ��� ��� ��������� ��� Y � ������� ���������
GF.Draw
'����� ������� ����� ���������� ����� ���. ���� ��� �� �������������� (������, ���� � ������ ���) ��������
GF.Repaint (Param)

'������ ������
GF.Gethering

End Sub