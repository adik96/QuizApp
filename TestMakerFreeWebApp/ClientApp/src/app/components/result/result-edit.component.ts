import { Component, Inject, OnInit } from "@angular/core";
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
@Component({
  selector: "result-edit",
  templateUrl: './result-edit.component.html',
  styleUrls: ['./result-edit.component.css']
})
export class ResultEditComponent {
  title: string;
  result: Result;
  form: FormGroup;
  // Otrzyma wartość TRUE w przypadku edycji istniejącego pytania
  // lub FALSE w przypadku nowego pytania
  editMode: boolean;
  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private fb: FormBuilder,
    @Inject('BASE_URL') private baseUrl: string) {
    // Utwórz pusty obiekt na podstawie interfejsu Result
    this.result = <Result>{};

    // initialize the form
    this.createForm();

    var id = +this.activatedRoute.snapshot.params["id"];
    // Sprawdź, czy znajdujemy się w trybie edycji
    this.editMode = (this.activatedRoute.snapshot.url[1].path === "edit");
    if (this.editMode) {
      // Pobierz pytanie z serwera
      var url = this.baseUrl + "api/result/" + id;
      this.http.get<Result>(url).subscribe(result => {
        this.result = result;
        this.title = "Edycja - " + this.result.Text;

        // update the form with the quiz value
        this.updateForm();
      }, error => console.error(error));
    }
    else {
      this.result.QuizId = id;
      this.title = "Utwórz nowe pytanie";
    }
  }

  createForm() {
    this.form = this.fb.group({
      Text: ['', Validators.required],
      MinValue: ['', Validators.pattern(/^\d*$/)],
      MaxValue: ['', Validators.pattern(/^\d*$/)]
    });
  }

  updateForm() {
    this.form.setValue({
      Text: this.result.Text,
      MinValue: this.result.MinValue,
      MaxValue: this.result.MaxValue
    });
  }

  onSubmit() {

    var tempResult = <Result>{};
    tempResult.Text = this.form.value.Text;
    tempResult.MinValue = this.form.value.MinValue;
    tempResult.MaxValue = this.form.value.MaxValue;
    tempResult.QuizId = this.result.QuizId;

    var url = this.baseUrl + "api/result";
    if (this.editMode) {
      this.http
        .post<Result>(url, tempResult)
        .subscribe(res => {
          var v = res;
          console.log("Wynik " + v.Id + " został uaktualniony.");
          this.router.navigate(["quiz/edit", v.QuizId]);
        }, error => console.log(error));
    }
    else {
      this.http
        .put<Result>(url, tempResult)
        .subscribe(res => {
          var v = res;
          console.log("Wynik " + v.Id + " został utworzony.");
          this.router.navigate(["quiz/edit", v.QuizId]);
        }, error => console.log(error));
    }
  }
  onBack() {
    this.router.navigate(["quiz/edit", this.result.QuizId]);
  }

  // retrieve a FormControl
  getFormControl(name: string) {
    return this.form.get(name);
  }

  // returns TRUE if the FormControl is valid
  isValid(name: string) {
    var e = this.getFormControl(name);
    return e && e.valid;
  }

  // returns TRUE if the FormControl has been changed
  isChanged(name: string) {
    var e = this.getFormControl(name);
    return e && (e.dirty || e.touched);
  }

  // returns TRUE if the FormControl is invalid after user changes
  hasError(name: string) {
    var e = this.getFormControl(name);
    return e && (e.dirty || e.touched) && !e.valid;
  }
}
