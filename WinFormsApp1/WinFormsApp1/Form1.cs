using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using DataTable = System.Data.DataTable;


namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "��ѡ���ļ�";
            openFileDialog.Filter = "Excel�ļ�(*.xlsx)|*.xlsx|�����ļ�(*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog.FileName;
                // �������TextBox�ؼ���ΪtextBox1
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.SelectionLength = 0; // ����ѡ�񳤶�Ϊ0����ʾû�����ֱ�ѡ��
                textBox1.Focus(); // ���ý��㵽textBox1
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "��ѡ���ļ���";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog.SelectedPath;
                // �������TextBox�ؼ���ΪtextBox1
                textBox2.SelectionStart = textBox2.Text.Length;
                textBox2.SelectionLength = 0; // ����ѡ�񳤶�Ϊ0����ʾû�����ֱ�ѡ��
                textBox2.Focus(); // ���ý��㵽textBox1
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string talbedesignbookpath = textBox1.Text;
                string sqlpath = textBox2.Text;
                DirectoryInfo directoryInfo = new DirectoryInfo(sqlpath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
                DataSet ds = this.ReadExcelToDataSet(talbedesignbookpath);
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.TableName.Contains("$_xlnm#Print_Area"))
                    {
                        string tableid = dt.Rows[1][2].ToString();
                        string tablename = dt.Rows[2][2].ToString();

                        FileInfo file = new FileInfo(sqlpath + "\\" + tableid + ".SQL");
                        using (StreamWriter sw = file.CreateText())
                        {
                            sw.WriteLine("/*");
                            sw.WriteLine("-- ���ɱ����������" + tablename);
                            sw.WriteLine("-- ���ɱ��Ӣ������" + tableid);
                            sw.WriteLine("-- ������֯����");
                            sw.WriteLine("-- �������ڣ�" + DateTime.Now.ToString("yyyy/MM/dd"));
                            sw.WriteLine("-- �����ߣ���ͼ����");
                            sw.WriteLine("-- �޸���ʷ��" + DateTime.Now.ToString("yyyy/MM/dd") + " ��ͼ���� �½��ļ�");
                            sw.WriteLine("*/");
                            sw.WriteLine("");
                            sw.WriteLine("------- ����ɾ�������������վ ----------");
                            sw.WriteLine("DROP TABLE " + tableid + " PURGE;");
                            sw.WriteLine("/");
                            sw.WriteLine("");
                            sw.WriteLine("------- ������ ----------");
                            sw.WriteLine("CREATE TABLE " + tableid + " (");

                            int maxcolumnidlen = 0;
                            List<ColumnObject> ColumnObjectList = new List<ColumnObject>();
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (dt.Rows[i][3] == null || dt.Rows[i][3].ToString().Trim().Length == 0)
                                {
                                    continue;
                                }

                                if (i >= 4)
                                {
                                    ColumnObject columnObject = new ColumnObject();
                                    // �к�����
                                    if (dt.Rows[i][2] != null && dt.Rows[i][2].ToString().Trim().Length > 0)
                                    {
                                        string columnname = dt.Rows[i][2].ToString();
                                        columnObject.Columnname = columnname;
                                    }
                                    // ��Ӣ����
                                    if (dt.Rows[i][3] != null && dt.Rows[i][3].ToString().Trim().Length > 0)
                                    {
                                        string columnid = dt.Rows[i][3].ToString();
                                        columnObject.Columnid = columnid;
                                        if (columnid.Length > maxcolumnidlen)
                                        {
                                            maxcolumnidlen = columnid.Length;
                                        }
                                    }
                                    // ����
                                    if (dt.Rows[i][4] != null && dt.Rows[i][4].ToString().Trim().Length > 0)
                                    {
                                        string columnkey = dt.Rows[i][4].ToString();
                                        columnObject.Columnkey = columnkey;
                                    }
                                    // �Ƿ�ǿ�
                                    if (dt.Rows[i][5] != null && dt.Rows[i][5].ToString().Trim().Length > 0)
                                    {
                                        string columnnullable = dt.Rows[i][5].ToString();
                                        columnObject.Columnnullable = columnnullable;
                                    }
                                    // ����
                                    if (dt.Rows[i][6] != null && dt.Rows[i][6].ToString().Trim().Length > 0)
                                    {
                                        string columntype = dt.Rows[i][6].ToString();
                                        columnObject.Columntype = columntype;
                                    }
                                    // ����λ
                                    if (dt.Rows[i][7] != null && dt.Rows[i][7].ToString().Trim().Length > 0)
                                    {
                                        string columninteger = dt.Rows[i][7].ToString();
                                        columnObject.Columninteger = columninteger;
                                    }
                                    // С��λ
                                    if (dt.Rows[i][8] != null && dt.Rows[i][8].ToString().Trim().Length > 0)
                                    {
                                        string columnxiaoshu = dt.Rows[i][8].ToString();
                                        columnObject.Columnxiaoshu = columnxiaoshu;
                                    }
                                    // Ĭ��ֵ
                                    if (dt.Rows[i][9] != null && dt.Rows[i][9].ToString().Trim().Length > 0)
                                    {
                                        string columndefault = dt.Rows[i][9].ToString();
                                        columnObject.Columndefault = columndefault;
                                    }
                                    // Լ������
                                    if (dt.Rows[i][10] != null && dt.Rows[i][10].ToString().Trim().Length > 0)
                                    {
                                        string columncheck = dt.Rows[i][10].ToString();
                                        columnObject.Columncheck = columncheck;
                                    }
                                    ColumnObjectList.Add(columnObject);
                                }
                            }

                            // ������
                            int index = 0;
                            string line = string.Empty;
                            foreach (ColumnObject co in ColumnObjectList)
                            {
                                line = string.Empty;

                                if (co.Columnid == null || co.Columnid.Length == 0)
                                {
                                    continue;
                                }

                                if (index == 0)
                                {
                                    line = line + "     " + co.Columnid.PadRight(maxcolumnidlen + 5);
                                }
                                else
                                {
                                    line = line + "    ," + co.Columnid.PadRight(maxcolumnidlen + 5);
                                }

                                if (co.Columntype == "VARCHAR2")
                                {
                                    line = line + " VARCHAR2(" + co.Columninteger + " CHAR)";
                                }
                                else if (co.Columntype == "NUMBER")
                                {
                                    if (co.Columnxiaoshu == null || co.Columnxiaoshu.Length == 0)
                                    {
                                        line = line + " NUMBER(" + co.Columninteger + ",0)";
                                    }
                                    else
                                    {
                                        line = line + " NUMBER(" + co.Columninteger + "," + co.Columnxiaoshu + ")";
                                    }
                                }
                                else if (co.Columntype == "DATE")
                                {
                                    line = line + " DATE";
                                }

                                if (co.Columndefault != null && co.Columndefault.Length > 0)
                                {
                                    line = line + " DEFAULT " + co.Columndefault;
                                }

                                if (co.Columnnullable != null && co.Columnnullable.Length > 0)
                                {
                                    line = line + " NOT NULL";
                                }

                                sw.WriteLine(line);

                                index = index + 1;
                            }
                            if (index >= 1)
                            {
                                sw.WriteLine(");");
                                sw.WriteLine("/");
                                sw.WriteLine("");
                            }

                            // ��Ӵ�����
                            sw.WriteLine("------- ��Ӵ����� ----------");
                            sw.WriteLine("CREATE OR REPLACE TRIGGER TRG_" + tableid + " BEFORE INSERT OR UPDATE ON " + tableid + " FOR EACH ROW");
                            sw.WriteLine("DECLARE");
                            sw.WriteLine("    LV_USER VARCHAR2(64 CHAR);");
                            sw.WriteLine("    LV_CLIENTID VARCHAR2(64 CHAR);");
                            sw.WriteLine("    LV_PROGRAMNAME VARCHAR2(50 CHAR);");
                            sw.WriteLine("    LV_TERMINALINAL VARCHAR2(50 CHAR);");
                            sw.WriteLine("BEGIN");
                            sw.WriteLine("    LV_USER := SYS_CONTEXT('USERENV', 'SESSION_USER');");
                            sw.WriteLine("    LV_CLIENTID:= SYS_CONTEXT('USERENV', 'CLIENT_INFO');");
                            sw.WriteLine("    LV_PROGRAMNAME:= SYS_CONTEXT('USERENV', 'CLIENT_PROGRAM_NAME');");
                            sw.WriteLine("    LV_TERMINALINAL:= SYS_CONTEXT('USERENV', 'TERMINAL');");
                            sw.WriteLine("");
                            sw.WriteLine("    IF NOT LV_CLIENTID IS NULL");
                            sw.WriteLine("        AND LENGTH(LV_CLIENTID) > 0");
                            sw.WriteLine("    THEN");
                            sw.WriteLine("        IF INSTR(LV_CLIENTID, ',') > 0");
                            sw.WriteLine("        THEN");
                            sw.WriteLine("            LV_USER := SUBSTR(LV_CLIENTID, 1, INSTR(LV_CLIENTID, ',') - 1);");
                            sw.WriteLine("            LV_PROGRAMNAME:= SUBSTR(LV_CLIENTID, INSTR(LV_CLIENTID, ',') + 1);");
                            sw.WriteLine("");
                            sw.WriteLine("            IF INSTR(LV_PROGRAMNAME, ',') > 0");
                            sw.WriteLine("            THEN");
                            sw.WriteLine("                LV_TERMINALINAL := SUBSTR(LV_PROGRAMNAME, INSTR(LV_PROGRAMNAME, ',') + 1);");
                            sw.WriteLine("                LV_PROGRAMNAME:= SUBSTR(LV_PROGRAMNAME, 1, INSTR(LV_PROGRAMNAME, ',') - 1);");
                            sw.WriteLine("            END IF;");
                            sw.WriteLine("        ELSE");
                            sw.WriteLine("            LV_USER := LV_CLIENTID;");
                            sw.WriteLine("        END IF;");
                            sw.WriteLine("    ELSE");
                            sw.WriteLine("        LV_USER := LV_USER || ',' || LV_PROGRAMNAME;");
                            sw.WriteLine("    END IF;");
                            sw.WriteLine("");
                            sw.WriteLine("    IF INSERTING");
                            sw.WriteLine("    THEN");
                            sw.WriteLine("        : new.INSTID := LV_USER;");
                            sw.WriteLine("        :new.INSTDT := sysdate;");
                            sw.WriteLine("        :new.INSTTERM := LV_TERMINALINAL;");
                            sw.WriteLine("        :new.INSTPRGNM := LV_PROGRAMNAME;");
                            sw.WriteLine("    END IF;");
                            sw.WriteLine("");
                            sw.WriteLine("    IF INSERTING OR UPDATING");
                            sw.WriteLine("    THEN");
                            sw.WriteLine("        : new.UPDTID := LV_USER;");
                            sw.WriteLine("        :new.UPDTDT := sysdate;");
                            sw.WriteLine("        :new.UPDTTERM := LV_TERMINALINAL;");
                            sw.WriteLine("        :new.UPDTPRGNM := LV_PROGRAMNAME;");
                            sw.WriteLine("    END IF;");
                            sw.WriteLine("END;");
                            sw.WriteLine("/");
                            sw.WriteLine("");

                            // �����������
                            sw.WriteLine("-------�����������----------");
                            index = 0;
                            line = "ALTER TABLE " + tableid + " ADD CONSTRAINT PK_" + tableid + " PRIMARY KEY (";
                            foreach (ColumnObject co in ColumnObjectList)
                            {
                                if (co.Columnkey != null && co.Columnkey.Length > 0)
                                {
                                    if (index == 0)
                                    {
                                        line = line + co.Columnid;
                                    }
                                    else
                                    {
                                        line = line + "," + co.Columnid;
                                    }
                                    index = index + 1;
                                }
                            }
                            line = line + ");";
                            if (index >= 1)
                            {
                                sw.WriteLine(line);
                                sw.WriteLine("/");
                                sw.WriteLine("");
                            }

                            // ���Լ������
                            sw.WriteLine("------- ���Լ������ ----------");
                            foreach (ColumnObject co in ColumnObjectList)
                            {
                                if (co.Columncheck != null && co.Columncheck.Length > 0)
                                {
                                    line = "ALTER TABLE " + tableid + " ADD CONSTRAINT CHECK_" + tableid + "_" + co.Columnid + "_1 CHECK(" + co.Columncheck + ");";
                                    sw.WriteLine(line);
                                    sw.WriteLine("/");
                                }
                            }
                            sw.WriteLine("");

                            // ��ӱ��������ע��
                            sw.WriteLine("------- ��ӱ��������ע�� ----------");
                            line = "COMMENT ON TABLE " + tableid + " IS '" + tablename + "';";
                            sw.WriteLine(line);
                            sw.WriteLine("/");
                            foreach (ColumnObject co in ColumnObjectList)
                            {
                                line = "COMMENT ON COLUMN " + tableid + "." + co.Columnid + " IS '" + co.Columnname + "';";
                                sw.WriteLine(line);
                                sw.WriteLine("/");
                            }
                        }
                    }
                }

                MessageBox.Show(sqlpath + Environment.NewLine + "SQL�ļ���������");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private DataSet ReadExcelToDataSet(string path)
        {
            //�����ַ���
            /* ��ע��
            	��� IMEX=1 ��ʾ�������е����ַ�����ȡ��ʵ��Ӧ�ò���������
            	ϵͳĬ�ϻ�鿴ǰ8��������ַ���������л�ʶ��Ϊ�ַ����С�
            	���ǰ8�ж������֣����ǻ�ʶ��Ϊ�����У�����Ҳһ����
            	��������8�в�������̫���ˣ���ֻ���޸�ע���HKEY_LOCAL_MACHINE/Software/Microsoft/Jet/4.0/Engines/Excel/TypeGuessRows��
            	�����ֵΪ0�����������������ж�ʹ��ʲô���ͣ�ͨ�����������������������������ȷʵ�Ƚ���
            */
            string connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 8.0;IMEX=1';";

            using (OleDbConnection conn = new OleDbConnection(connstring))
            {
                conn.Open();
                DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });//������е�sheet
                DataSet set = new DataSet();
                for (int i = 0; i < sheetsName.Rows.Count; i++)
                {
                    string sheetName = sheetsName.Rows[i][2].ToString();
                    string sql = string.Format("SELECT * FROM [{0}]", sheetName);
                    OleDbDataAdapter ada = new OleDbDataAdapter(sql, connstring);

                    ada.Fill(set);
                    set.Tables[i].TableName = sheetName;
                }

                return set;

            }
        }
    }

    public class ColumnObject
    {
        // �к�����
        private string columnname;
        // ��Ӣ����
        private string columnid;
        // ����
        private string columnkey;
        // �Ƿ�ǿ�
        private string columnnullable;
        // ����
        private string columntype;
        // ����λ
        private string columninteger;
        // С��λ
        private string columnxiaoshu;
        // Ĭ��ֵ
        private string columndefault;
        // Լ������
        private string columncheck;

        public string Columnname { get => columnname; set => columnname = value; }
        public string Columnid { get => columnid; set => columnid = value; }
        public string Columnkey { get => columnkey; set => columnkey = value; }
        public string Columnnullable { get => columnnullable; set => columnnullable = value; }
        public string Columntype { get => columntype; set => columntype = value; }
        public string Columninteger { get => columninteger; set => columninteger = value; }
        public string Columnxiaoshu { get => columnxiaoshu; set => columnxiaoshu = value; }
        public string Columndefault { get => columndefault; set => columndefault = value; }
        public string Columncheck { get => columncheck; set => columncheck = value; }
    }
}