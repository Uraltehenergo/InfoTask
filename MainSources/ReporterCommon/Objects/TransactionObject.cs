namespace ReporterCommon
{
    //������� ����� ��� TransactionCell � TransactionShape
    public abstract class TransactionObject
    {
        //������ � ����� �������� ������
        public string OldValue { get; private set; }
        public string NewValue { get; set; }
        //������ � ����� ���������� � ������
        public string OldLink { get; private set; }
        public string NewLink { get; set; }

        //������ � ������ �������� 
        public abstract string Value { get; set; }
        //������ � ������ �����������  
        public abstract string Link { get; set; }

        //��������� ������
        protected void SaveOldValue()
        {
            try { NewLink = OldLink = Link; } catch {}
            try { NewValue = OldValue = Value; } catch { }
        }

        //����� ����������
        public void Undo()
        {
            try { if (OldLink != NewLink) Link = OldLink; } catch { }
            try { if (OldValue != NewValue) Value = OldValue; } catch { }
        }

        //�������� ����� ����������
        public void Redo()
        {
            try { if (OldLink != NewLink) Link = NewLink; } catch { }
            try { if (OldValue != NewValue) Value = NewValue; } catch { }
        }
    }
}