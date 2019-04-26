import { Component, Inject, OnInit } from "@angular/core";
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { Query } from "@angular/compiler/src/core";
@Component({
  selector: "question-edit",
  templateUrl: './question-edit.component.html',
  styleUrls: ['./question-edit.component.css']
})
export class QuestionEditComponent {
  title: string;
  question: Question;
  form: FormGroup;
  // Otrzyma wartość TRUE w przypadku edycji istniejącego pytania
  // lub FALSE w przypadku nowego pytania
  editMode: boolean;
  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private fb: FormBuilder,
    @Inject('BASE_URL') private baseUrl: string) {
    // Utwórz pusty obiekt na podstawie interfejsu Question
    this.question = <Question>{};

    this.createForm();

    var id = +this.activatedRoute.snapshot.params["id"];
    // Sprawdź, czy znajdujemy się w trybie edycji
    this.editMode = (this.activatedRoute.snapshot.url[1].path === "edit");
    if (this.editMode) {
      // Pobierz pytanie z serwera
      var url = this.baseUrl + "api/question/" + id;
      this.http.get<Question>(url).subscribe(result => {
        this.question = result;
        this.title = "Edycja - " + this.question.Text;

        this.updateForm();

      }, error => console.error(error));
    }
    else {
      this.question.QuizId = id;
      this.title = "Utwórz nowe pytanie";
    }
  }

  createForm() {
    this.form = this.fb.group({
      Text: ['', Validators.required]
    });
  }

  updateForm() {
    this.form.setValue({
      Text: this.question.Text
    });
  }

  getFormControl(name: string) {
    return this.form.get(name);
  }

  isValid(name: string) {
    var e = this.getFormControl(name);
    return e && e.valid;
  }

  isChanged(name: string) {
    var e = this.getFormControl(name);
    return e && (e.dirty || e.touched);
  }

  hasError(name: string) {
    var e = this.getFormControl(name);
    return e && (e.dirty || e.touched) && !e.valid;
  }

  onSubmit() {

    var tempQuestion = <Question>{};
    tempQuestion.Text = this.form.value.Text;

    var url = this.baseUrl + "api/question";
    if (this.editMode) {

      tempQuestion.Id = this.question.Id;

      this.http
        .post<Question>(url, tempQuestion)
        .subscribe(res => {
          var v = res;
          console.log("Pytanie " + v.Id + " zostało uaktualnione.");
          this.router.navigate(["quiz/edit", v.QuizId]);
        }, error => console.log(error));
    }
    else {
      this.http
        .put<Question>(url, tempQuestion)
        .subscribe(res => {
          var v = res;
          console.log("Pytanie " + v.Id + " zostało utworzone.");
          this.router.navigate(["quiz/edit", v.QuizId]);
        }, error => console.log(error));
    }
  }
  onBack() {
    this.router.navigate(["quiz/edit", this.question.QuizId]);
  }
}
