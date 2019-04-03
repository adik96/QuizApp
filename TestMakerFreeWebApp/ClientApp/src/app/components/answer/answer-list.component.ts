import { Component, Inject, Input, OnChanges, SimpleChanges } from "@angular/core";
import { Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
@Component({
  selector: "answer-list",
  templateUrl: './answer-list.component.html',
  styleUrls: ['./answer-list.component.css']
})
export class AnswerListComponent implements OnChanges {
  @Input() question: Question;
  answers: Answer[];
  title: string;
  constructor(private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private router: Router) {
    this.answers = [];
  }
  ngOnChanges(changes: SimpleChanges) {
    if (typeof changes['question'] !== "undefined") {
      // Pobierz informacje zawarte pod kluczem question
      var change = changes['question'];
      // Wykonaj zadanie tylko w sytuacji, gdy wartość uległa zmianie
      if (!change.isFirstChange()) {
        // Wykonaj żądanie HTTP i pobierz wynik
        this.loadData();
      }
    }
  }
  loadData() {
    var url = this.baseUrl + "api/answer/All/" + this.question.Id;
    this.http.get<Answer[]>(url).subscribe(result => {
      this.answers = result;
    }, error => console.error(error));
  }
  onCreate() {
    this.router.navigate(["/answer/create", this.question.Id]);
  }
  onEdit(answer: Answer) {
    this.router.navigate(["/answer/edit", answer.Id]);
  }
  onDelete(answer: Answer) {
    if (confirm("Czy naprawdę chcesz usunąć tę odpowiedź?")) {
      var url = this.baseUrl + "api/answer/" + answer.Id;
      this.http
        .delete(url)
        .subscribe(res => {
          console.log("Odpowiedź " + answer.Id + " została usunięta.");
          // Odśwież listę
          this.loadData();
        }, error => console.log(error));
    }
  }
}
