import { Component, Inject, OnInit } from "@angular/core";
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
@Component({
  selector: "answer-edit",
  templateUrl: './answer-edit.component.html',
  styleUrls: ['./answer-edit.component.css']
})
export class AnswerEditComponent {
  title: string;
  answer: Answer;
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
    this.answer = <Answer>{};

    this.createForm();

    var id = +this.activatedRoute.snapshot.params["id"];
    // Sprawdź, czy znajdujemy się w trybie edycji
    this.editMode = (this.activatedRoute.snapshot.url[1].path === "edit");
    if (this.editMode) {
      // Pobierz odpowiedzi z serwera
      var url = this.baseUrl + "api/answer/" + id;
      this.http.get<Answer>(url).subscribe(result => {
        this.answer = result;
        this.title = "Edycja - " + this.answer.Text;

        this.updateForm();

      }, error => console.error(error));
    }
    else {
      this.answer.QuestionId = id;
      this.title = "Utwórz nową odpowiedź";
    }
  }

  createForm() {
    this.form = this.fb.group({
      Text: ['', Validators.required],
      Value: ['',
        [Validators.required,
          Validators.min(-5),
          Validators.max(5)]
      ]
    });
  }

  updateForm() {
    this.form.setValue({
      Text: this.answer.Text || '',
      Value: this.answer.Value || 0
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
    // build a temporary answer object from form values
    var tempAnswer = <Answer>{};
    tempAnswer.Text = this.form.value.Text;
    tempAnswer.Value = this.form.value.Value;
    tempAnswer.QuestionId = this.answer.QuestionId;

    var url = this.baseUrl + "api/answer";
    if (this.editMode) {
      // don't forget to set the tempAnswer Id,
      //   otherwise the EDIT would fail!
      tempAnswer.Id = this.answer.Id;

      this.http
        .post<Answer>(url, tempAnswer)
        .subscribe(res => {
          var v = res;
          console.log("Odpowiedź " + v.Id + " została uaktualniona.");
          this.router.navigate(["question/edit", v.QuestionId]);
        }, error => console.log(error));
    }
    else {
      this.http
        .put<Answer>(url, tempAnswer)
        .subscribe(res => {
          var v = res;
          console.log("Odpowiedź " + v.Id + " została utworzona.");
          this.router.navigate(["question/edit", v.QuestionId]);
        }, error => console.log(error));
    }
  }
  onBack() {
    this.router.navigate(["question/edit", this.answer.QuestionId]);
  }
}
