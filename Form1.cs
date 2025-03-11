using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp7
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public abstract class Person
        {
            public string ID { get; set; }
            public string Name { get; set; }

            public abstract override string ToString();
        }

        public class Student : Person
        {
            public string Major { get; set; }
            public string AdvisorName { get; set; }
            public double GPA { get; set; }

            public override string ToString()
            {
                return $"{ID} - {Name} ({Major}) - GPA: {GPA}, Advisor: {AdvisorName}";
            }
        }

        public class Teacher : Person
        {
            private List<Student> advisees = new List<Student>();

            public string Major { get; set; }

            public void AddAdvisee(Student student)
            {
                advisees.Add(student);
            }

            public List<Student> GetAdvisees()
            {
                return advisees;
            }

            public override string ToString()
            {
                return $"{ID} - {Name} ({Major})";
            }
        }

        private List<Student> students = new List<Student>();
        private List<Teacher> teachers = new List<Teacher>();

        // กดปุ่ม button1 เพื่อบันทึกข้อมูลนักศึกษา
        private void button1Click(object sender, EventArgs e)
        {
            StudentClick(sender, e);
        }

        // เพิ่มนักศึกษา
        private void StudentClick(object sender, EventArgs e)
        {
            string id = txtID.Text.Trim();
            string name = txtName.Text.Trim();
            string major = txtMajor.Text.Trim();
            string advisorID = txtAdvisor.Text.Trim();
            double gpa;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(major) || string.IsNullOrEmpty(advisorID) || !double.TryParse(txtGPA.Text, out gpa))
            {
                MessageBox.Show("Please fill in all fields correctly, especially GPA.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // สร้าง Student object และบันทึกข้อมูล
            Student student = new Student { ID = id, Name = name, Major = major, GPA = gpa, AdvisorName = advisorID };

            // เพิ่มนักศึกษาใน List
            students.Add(student);

            // แสดงผลใน ListBox
            lstStudents.Items.Add(student.ToString());

            // ตรวจสอบว่ามีอาจารย์ที่ปรึกษาหรือไม่
            var advisor = teachers.FirstOrDefault(t => t.ID == advisorID);
            if (advisor != null)
            {
                advisor.AddAdvisee(student);
            }
            else
            {
                MessageBox.Show("Advisor not found!", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // ล้างค่า input
            txtID.Clear();
            txtName.Clear();
            txtMajor.Clear();
            txtAdvisor.Clear();
            txtGPA.Clear();
        }

        // เพิ่มอาจารย์ที่ปรึกษา
        private void TeacherClick(object sender, EventArgs e)
        {
            string id = txtTeacherID.Text.Trim();
            string name = txtTeacherName.Text.Trim();
            string major = txtTeacherMajor.Text.Trim();

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(major))
            {
                MessageBox.Show("Please fill in all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!teachers.Any(t => t.ID == id)) // ป้องกัน ID ซ้ำ
            {
                Teacher newTeacher = new Teacher { ID = id, Name = name, Major = major };
                teachers.Add(newTeacher);
                lstTeachers.Items.Add(newTeacher.ToString());
            }
            else
            {
                MessageBox.Show("Teacher already exists with this ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // แสดงนักศึกษาที่ได้ GPA สูงสุด
        private void showGPA_Click(object sender, EventArgs e)
        {
            if (students.Count == 0)
            {
                MessageBox.Show("No students to display.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var topStudent = students.OrderByDescending(s => s.GPA).First();
            lblTopStudent.Text = $"{topStudent.Name} ({topStudent.GPA})";
        }

        // แสดงนักศึกษาที่อยู่ในความดูแลของอาจารย์ที่ปรึกษา
        private void lstTeachers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTeachers.SelectedIndex >= 0)
            {
                string selectedText = lstTeachers.SelectedItem.ToString();
                string teacherID = selectedText.Split('-')[0].Trim();

                var selectedTeacher = teachers.FirstOrDefault(t => t.ID == teacherID);
                if (selectedTeacher != null)
                {
                    var advisees = selectedTeacher.GetAdvisees();
                    string adviseeList = advisees.Count > 0 ? string.Join("\n", advisees.Select(s => s.ToString())) : "No advisees found.";
                    MessageBox.Show(adviseeList, $"Advisees of {selectedTeacher.Name}");
                }
            }
        }
    }
}
