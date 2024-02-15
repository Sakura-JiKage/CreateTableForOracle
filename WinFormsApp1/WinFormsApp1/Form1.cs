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
            openFileDialog.Title = "请选择文件";
            openFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog.FileName;
                // 假设你的TextBox控件名为textBox1
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.SelectionLength = 0; // 设置选择长度为0，表示没有文字被选中
                textBox1.Focus(); // 设置焦点到textBox1
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "请选择文件夹";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog.SelectedPath;
                // 假设你的TextBox控件名为textBox1
                textBox2.SelectionStart = textBox2.Text.Length;
                textBox2.SelectionLength = 0; // 设置选择长度为0，表示没有文字被选中
                textBox2.Focus(); // 设置焦点到textBox1
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
                            sw.WriteLine("-- 作成表的中文名：" + tablename);
                            sw.WriteLine("-- 作成表的英文名：" + tableid);
                            sw.WriteLine("-- 作成组织：无");
                            sw.WriteLine("-- 作成日期：" + DateTime.Now.ToString("yyyy/MM/dd"));
                            sw.WriteLine("-- 作成者：烟图黛螺");
                            sw.WriteLine("-- 修改历史：" + DateTime.Now.ToString("yyyy/MM/dd") + " 烟图黛螺 新建文件");
                            sw.WriteLine("*/");
                            sw.WriteLine("");
                            sw.WriteLine("------- 永久删除表，不进入回收站 ----------");
                            sw.WriteLine("DROP TABLE " + tableid + " PURGE;");
                            sw.WriteLine("/");
                            sw.WriteLine("");
                            sw.WriteLine("------- 创建表 ----------");
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
                                    // 列汉字名
                                    if (dt.Rows[i][2] != null && dt.Rows[i][2].ToString().Trim().Length > 0)
                                    {
                                        string columnname = dt.Rows[i][2].ToString();
                                        columnObject.Columnname = columnname;
                                    }
                                    // 列英文名
                                    if (dt.Rows[i][3] != null && dt.Rows[i][3].ToString().Trim().Length > 0)
                                    {
                                        string columnid = dt.Rows[i][3].ToString();
                                        columnObject.Columnid = columnid;
                                        if (columnid.Length > maxcolumnidlen)
                                        {
                                            maxcolumnidlen = columnid.Length;
                                        }
                                    }
                                    // 主键
                                    if (dt.Rows[i][4] != null && dt.Rows[i][4].ToString().Trim().Length > 0)
                                    {
                                        string columnkey = dt.Rows[i][4].ToString();
                                        columnObject.Columnkey = columnkey;
                                    }
                                    // 是否非空
                                    if (dt.Rows[i][5] != null && dt.Rows[i][5].ToString().Trim().Length > 0)
                                    {
                                        string columnnullable = dt.Rows[i][5].ToString();
                                        columnObject.Columnnullable = columnnullable;
                                    }
                                    // 类型
                                    if (dt.Rows[i][6] != null && dt.Rows[i][6].ToString().Trim().Length > 0)
                                    {
                                        string columntype = dt.Rows[i][6].ToString();
                                        columnObject.Columntype = columntype;
                                    }
                                    // 整数位
                                    if (dt.Rows[i][7] != null && dt.Rows[i][7].ToString().Trim().Length > 0)
                                    {
                                        string columninteger = dt.Rows[i][7].ToString();
                                        columnObject.Columninteger = columninteger;
                                    }
                                    // 小数位
                                    if (dt.Rows[i][8] != null && dt.Rows[i][8].ToString().Trim().Length > 0)
                                    {
                                        string columnxiaoshu = dt.Rows[i][8].ToString();
                                        columnObject.Columnxiaoshu = columnxiaoshu;
                                    }
                                    // 默认值
                                    if (dt.Rows[i][9] != null && dt.Rows[i][9].ToString().Trim().Length > 0)
                                    {
                                        string columndefault = dt.Rows[i][9].ToString();
                                        columnObject.Columndefault = columndefault;
                                    }
                                    // 约束条件
                                    if (dt.Rows[i][10] != null && dt.Rows[i][10].ToString().Trim().Length > 0)
                                    {
                                        string columncheck = dt.Rows[i][10].ToString();
                                        columnObject.Columncheck = columncheck;
                                    }
                                    ColumnObjectList.Add(columnObject);
                                }
                            }

                            // 创建表
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

                            // 添加触发器
                            sw.WriteLine("------- 添加触发器 ----------");
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

                            // 添加主键索引
                            sw.WriteLine("-------添加主键索引----------");
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

                            // 添加约束条件
                            sw.WriteLine("------- 添加约束条件 ----------");
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

                            // 添加表和列名的注释
                            sw.WriteLine("------- 添加表和列名的注释 ----------");
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

                MessageBox.Show(sqlpath + Environment.NewLine + "SQL文件生成完了");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private DataSet ReadExcelToDataSet(string path)
        {
            //连接字符串
            /* 备注：
            	添加 IMEX=1 表示将所有列当做字符串读取，实际应该不是这样，
            	系统默认会查看前8行如果有字符串，则该列会识别为字符串列。
            	如果前8行都是数字，则还是会识别为数字列，日期也一样；
            	如果你觉得8行不够或者太多了，则只能修改注册表HKEY_LOCAL_MACHINE/Software/Microsoft/Jet/4.0/Engines/Excel/TypeGuessRows，
            	如果此值为0，则会根据所有行来判断使用什么类型，通常不建议这麽做，除非你的数据量确实比较少
            */
            string connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 8.0;IMEX=1';";

            using (OleDbConnection conn = new OleDbConnection(connstring))
            {
                conn.Open();
                DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });//存放所有的sheet
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
        // 列汉字名
        private string columnname;
        // 列英文名
        private string columnid;
        // 主键
        private string columnkey;
        // 是否非空
        private string columnnullable;
        // 类型
        private string columntype;
        // 整数位
        private string columninteger;
        // 小数位
        private string columnxiaoshu;
        // 默认值
        private string columndefault;
        // 约束条件
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