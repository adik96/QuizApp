import { Component, Inject, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
@Component({
  selector: "quiz-edit",
  templateUrl: './quiz-edit.component.html',
  styleUrls: ['./quiz-edit.component.css']
})
export class QuizEditComponent {
  title: string;
  quiz: Quiz;
  // Otrzyma wartość TRUE w przypadku edycji istniejącego quizu
  // lub FALSE w przypadku nowego quizu
  editMode: boolean;
  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string) {
    // Utwórz pusty obiekt zgodny z interfejsem Quiz
    this.quiz = <Quiz>{};
    var id = +this.activatedRoute.snapshot.params["id"];
    if (id) {
      this.editMode = true;
      // Pobierz quiz z serwera
      var url = this.baseUrl + "api/quiz/" + id;
      this.http.get<Quiz>(url).subscribe(result => {
        this.quiz = result;
        this.title = "Edycja - " + this.quiz.Title;
      }, error => console.error(error));
    }
    else {
      this.editMode = false;
      this.title = "Utwórz nowy quiz";
    }
  }
  onSubmit(quiz: Quiz) {
    var url = this.baseUrl + "api/quiz";
    if (this.editMode) {
      this.http
        .post<Quiz>(url, quiz)
        .subscribe(result => {
          var v = result;
          console.log("Quiz " + v.Id + " został uaktualniony.");
          this.router.navigate(["home"]);
        }, error => console.log(error));
    }
    else {
      this.http
        .put<Quiz>(url, quiz)
        .subscribe(result => {
          var v = result;
          console.log("Quiz " + v.Id + " został utworzony.");
          this.router.navigate(["home"]);
        }, error => console.log(error));
    }
  }
  onBack() {
    this.router.navigate(["home"]);
  }
}
