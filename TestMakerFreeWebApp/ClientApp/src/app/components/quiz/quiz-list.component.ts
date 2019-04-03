import { Component, Inject, Input, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";

@Component({
  selector: "quiz-list",
  templateUrl: './quiz-list.component.html',
  styleUrls: ['./quiz-list.component.css']
})
export class QuizListComponent implements OnInit {
  @Input() class: string;
  title: string;
  selectedQuiz: Quiz;
  quizzes: Quiz[];
  //http: HttpClient;
  //baseUrl: string;

  constructor(private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router) {
    // this.http = http;
    //this.baseUrl = baseUrl;
  }

  ngOnInit() {
      var url = this.baseUrl + "api/quiz/";
      console.log("QuizListComponent " +
        " został utworzony z użyciem następującej klasy: "
        + this.class);
      switch (this.class) {
        case "latest":
        default:
          this.title = "Najnowsze quizy";
          url += "Latest/";
          break;
        case "byTitle":
          this.title = "Quizy alfabetycznie";
          url += "ByTitle/";
          break;
        case "random":
          this.title = "Losowe quizy";
          url += "Random/";
          break;
      }


      this.http.get<Quiz[]>(url).subscribe(result => {
        this.quizzes = result;
      }, error => console.error(error));
    }
  

  onSelect(quiz: Quiz) {
    this.selectedQuiz = quiz;
    console.log("Wybrano quiz o identyfikatorze "
      + this.selectedQuiz.Id);
    this.router.navigate(["quiz", this.selectedQuiz.Id]);
  }
}
