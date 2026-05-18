// Question type constants (must match QuestionType enum in C#)
const QT = { MultipleChoice: 0, SingleChoice: 1, FillInTheBlank: 2, OpenWithImage: 3 };

function addQuestion() {
  const container = document.getElementById("questions-container");

  const usedIndices = [
    ...container.querySelectorAll("input[name^='Questions'][name$='.Index']"),
  ]
    .map((input) => parseInt(input.value))
    .filter((n) => !isNaN(n));
  const bindingIndex =
    usedIndices.length > 0 ? Math.max(...usedIndices) + 1 : 0;

  const displayNumber = container.querySelectorAll(".question-box").length + 1;

  const questionDiv = document.createElement("div");
  questionDiv.className = "mb-4 border p-3 rounded bg-light question-box";
  questionDiv.id = `question-${bindingIndex}`;
  questionDiv.setAttribute("data-question-index", bindingIndex);

  questionDiv.innerHTML = `
    <button type="button" class="btn btn-danger btn-sm mb-2" onclick="removeQuestion(${bindingIndex})">Remove</button>
    <h5>Question ${displayNumber}</h5>

    <input type="hidden" name="Questions[${bindingIndex}].QuestionId" value="0" />
    <input type="hidden" name="Questions[${bindingIndex}].Index" value="${bindingIndex}" />

    <div class="form-group mb-2">
      <label>Type</label>
      <select name="Questions[${bindingIndex}].Type" class="form-control"
              onchange="onQuestionTypeChange(this, ${bindingIndex})">
        <option value="${QT.MultipleChoice}">Multiple choice (checkboxes)</option>
        <option value="${QT.SingleChoice}">Single choice (radio)</option>
        <option value="${QT.FillInTheBlank}">Fill in the blank</option>
        <option value="${QT.OpenWithImage}">Open with image</option>
      </select>
    </div>

    <div class="form-group mb-2">
      <label>Description</label>
      <input name="Questions[${bindingIndex}].Description" class="form-control" required />
    </div>
    <div class="form-group mb-2">
      <label>Score</label>
      <input type="number" name="Questions[${bindingIndex}].QuestionScore" class="form-control" value="1" required />
    </div>

    <div id="type-fields-${bindingIndex}">
      <!-- Rendered by onQuestionTypeChange -->
    </div>
  `;

  container.appendChild(questionDiv);
  // Render default type fields (MultipleChoice)
  const sel = questionDiv.querySelector(`select[name="Questions[${bindingIndex}].Type"]`);
  onQuestionTypeChange(sel, bindingIndex);
}

function onQuestionTypeChange(selectEl, qIdx) {
  const type = parseInt(selectEl.value);
  const container = document.getElementById(`type-fields-${qIdx}`);
  container.innerHTML = "";

  if (type === QT.MultipleChoice || type === QT.SingleChoice) {
    container.innerHTML = `
      <div class="form-group mb-2">
        <label>Number of answers</label>
        <input type="number" min="2" max="10" class="form-control" style="max-width:120px;"
               placeholder="e.g. 4" onchange="addAnswers(this, ${qIdx}, ${type})" />
      </div>
      <div id="answers-${qIdx}" class="mt-2"></div>
    `;
  } else if (type === QT.FillInTheBlank) {
    container.innerHTML = `
      <div class="form-group mb-2">
        <label>Accepted answers <small class="text-muted">(comma-separated, case-insensitive)</small></label>
        <input type="text" name="Questions[${qIdx}].Keywords" class="form-control"
               placeholder="e.g. Paris, paris, PARIS" />
      </div>
    `;
  } else if (type === QT.OpenWithImage) {
    container.innerHTML = `
      <div class="form-group mb-2">
        <label>Question image</label>
        <input type="file" accept="image/*" class="form-control"
               onchange="uploadQuestionImage(this, ${qIdx})" />
        <input type="hidden" name="Questions[${qIdx}].ImagePath" id="img-path-${qIdx}" />
        <div id="img-preview-${qIdx}" class="mt-2"></div>
      </div>
      <div class="form-group mb-2">
        <label>Grading method</label>
        <select name="Questions[${qIdx}].Grading" class="form-control"
                onchange="onGradingMethodChange(this, ${qIdx})">
          <option value="0">Keywords (automatic)</option>
          <option value="1">Manual (creator reviews)</option>
        </select>
      </div>
      <div id="keywords-field-${qIdx}">
        <div class="form-group mb-2">
          <label>Accepted keywords <small class="text-muted">(comma-separated, case-insensitive)</small></label>
          <input type="text" name="Questions[${qIdx}].Keywords" class="form-control"
                 placeholder="e.g. Eiffel Tower, eiffel tower" />
        </div>
      </div>
    `;
  }
}

function onGradingMethodChange(selectEl, qIdx) {
  const method = parseInt(selectEl.value);
  const kf = document.getElementById(`keywords-field-${qIdx}`);
  if (!kf) return;
  kf.style.display = method === 0 ? "" : "none";
}

// Preserve existing hidden AnswerId values when switching type to avoid accidental data loss
// (they'll be ignored if the type has no answers, but they stay in DOM)

async function uploadQuestionImage(inputEl, qIdx) {
  const file = inputEl.files[0];
  if (!file) return;

  const formData = new FormData();
  formData.append("file", file);

  const resp = await fetch("/Quiz/UploadQuestionImage", { method: "POST", body: formData });
  if (!resp.ok) { alert("Image upload failed."); return; }

  const { path } = await resp.json();
  document.getElementById(`img-path-${qIdx}`).value = path;

  const preview = document.getElementById(`img-preview-${qIdx}`);
  preview.innerHTML = `<img src="/Quiz/QuestionImage?path=${encodeURIComponent(path)}"
                            style="max-height:180px;border-radius:6px;" alt="preview" />`;
}

function addAnswers(input, qIdx, type) {
  const count = parseInt(input.value);
  const answersContainer = document.getElementById(`answers-${qIdx}`);
  answersContainer.innerHTML = "";

  const inputType = type === QT.SingleChoice ? "radio" : "checkbox";

  for (let i = 0; i < count; i++) {
    const wrapper = document.createElement("div");
    wrapper.className = "form-group mb-2";
    wrapper.innerHTML = `
      <label>Answer ${i + 1}</label>
      <input name="Questions[${qIdx}].Answers[${i}].Description" class="form-control mb-1" required />
      <div class="form-check">
        <input class="form-check-input" type="${inputType}"
               name="Questions[${qIdx}].Answers[${i}].IsCorrect"
               value="true" id="correct-${qIdx}-${i}"
               ${type === QT.SingleChoice ? `onclick="enforceRadioCorrect(this, ${qIdx})"` : ""}>
        <label class="form-check-label" for="correct-${qIdx}-${i}">Mark as correct</label>
      </div>`;
    answersContainer.appendChild(wrapper);
  }
}

// For SingleChoice, only one answer can be marked correct
function enforceRadioCorrect(changedCheckbox, qIdx) {
  const container = document.getElementById(`answers-${qIdx}`);
  container.querySelectorAll(`input[type="radio"][name$=".IsCorrect"]`).forEach(cb => {
    if (cb !== changedCheckbox) cb.checked = false;
  });
}

function removeQuestion(questionIndex) {
  const container = document.getElementById("questions-container");
  const q = container.querySelector(`[data-question-index="${questionIndex}"]`);
  if (!q) return;
  q.remove();

  const inputsToRemove = container.querySelectorAll(
    `input[name^="Questions[${questionIndex}]"],
     select[name^="Questions[${questionIndex}]"],
     textarea[name^="Questions[${questionIndex}]"]`
  );
  inputsToRemove.forEach((el) => el.remove());

  const allInputs = container.querySelectorAll("input[type='hidden']");
  allInputs.forEach((input) => {
    const name = input.getAttribute("name");
    if (name && name.startsWith(`Questions[${questionIndex}].`)) input.remove();
  });

  const inputsToUpdate = container.querySelectorAll("[name]");
  inputsToUpdate.forEach((input) => {
    const name = input.getAttribute("name");
    const match = name.match(/^Questions\[(\d+)\]\.(.+)$/);
    if (match) {
      const currentIndex = parseInt(match[1]);
      const field = match[2];
      if (currentIndex > questionIndex) {
        input.setAttribute("name", `Questions[${currentIndex - 1}].${field}`);
      }
    }
  });

  const allQuestions = container.querySelectorAll("[data-question-index]");
  allQuestions.forEach((el, i) => {
    const oldIndex = parseInt(el.getAttribute("data-question-index"));
    if (oldIndex > questionIndex) {
      const newIndex = oldIndex - 1;
      el.setAttribute("data-question-index", newIndex);
      el.id = `question-${newIndex}`;
    }
    const title = el.querySelector("h5");
    if (title) title.textContent = `Question ${i + 1}`;
  });
}
