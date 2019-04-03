import { Component, Inject, OnInit } from "@angular/core";
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
  // Otrzyma wartość TRUE w przypadku edycji istniejącego pytania
  // lub FALSE w przypadku nowego pytania
  editMode: boolean;
  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string) {
    // Utwórz pusty obiekt na podstawie interfejsu Result
    this.result = <Result>{};
    var id = +this.activatedRoute.snapshot.params["id"];
    // Sprawdź, czy znajdujemy się w trybie edycji
    this.editMode = (this.activatedRoute.snapshot.url[1].path === "edit");
    if (this.editMode) {
      // Pobierz pytanie z serwera
      var url = this.baseUrl + "api/result/" + id;
      this.http.get<Result>(url).subscribe(result => {
        this.result = result;
        this.title = "Edycja - " + this.result.Text;
      }, error => console.error(error));
    }
    else {
      this.result.QuizId = id;
      this.title = "Utwórz nowe pytanie";
    }
  }
  onSubmit(result: Result) {
    var url = this.baseUrl + "api/result";
    if (this.editMode) {
      this.http
        .post<Result>(url, result)
        .subscribe(res => {
          var v = res;
          console.log("Wynik " + v.Id + " zostało uaktualnione.");
          this.router.navigate(["quiz/edit", v.QuizId]);
        }, error => console.log(error));
    }
    else {
      this.http
        .put<Result>(url, result)
        .subscribe(res => {
          var v = res;
          console.log("Wynik " + v.Id + " zostało utworzone.");
          this.router.navigate(["quiz/edit", v.QuizId]);
        }, error => console.log(error));
    }
  }
  onBack() {
    this.router.navigate(["quiz/edit", this.result.QuizId]);
  }
}
