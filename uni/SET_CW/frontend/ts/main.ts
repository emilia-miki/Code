enum AdFormat {
  MagazineText,
  MagazineTextWithMedia,
  MagazinePhoto,
  Billboard,
  WebVideo,
  WebImage,
}

interface Template {
  id: string;
  html_id: string;
  style_ids: string[];
  format: AdFormat;
  price: number;
}

interface TemplateInDB {
  template: Template;
}

interface Base {
  id: string;
  content: string;
}

interface HTML extends Base {}

interface HTMLInDB {
  html: HTML;
}

interface Style extends Base {}

interface StyleInDB {
  style: Style;
}

type FormNull = HTMLFormElement | null;
type SelectNullUndefined = HTMLSelectElement | null | undefined;
type SelectInput = HTMLSelectElement | HTMLInputElement;

// Template (POST)
const templatePostForm: FormNull = document.querySelector('[name="templatePost"]');

// Template (PUT)
const templatePutForm: FormNull = document.querySelector('[name="templatePut"]');
const templateSelectPut: SelectNullUndefined = templatePutForm?.querySelector('[name="id"]');

// Template (DELETE)
const templateDeleteForm: FormNull = document.querySelector('[name="templateDelete"]');

// HTML (POST)
const htmlPostForm: FormNull = document.querySelector('[name="htmlPost"]');

// HTML (PUT)
const htmlPutForm: FormNull = document.querySelector('[name="htmlPut"]');
const htmlSelectPut: SelectNullUndefined = htmlPutForm?.querySelector('[name="id"]');

// HTML (DELETE)
const htmlDeleteForm: FormNull = document.querySelector('[name="htmlDelete"]');

// Style (POST)
const stylePostForm: FormNull = document.querySelector('[name="stylePost"]');

// Style (PUT)
const stylePutForm: FormNull = document.querySelector('[name="stylePut"]');
const styleSelectPut: SelectNullUndefined = stylePutForm?.querySelector('[name="id"]');

// Style (DELETE)
const styleDeleteForm: FormNull = document.querySelector('[name="styleDelete"]');

// Download htmls
(async function(): Promise<void> {
  const templateHTMLSelectPost: SelectNullUndefined = templatePostForm?.querySelector('[name="html_id"]');
  const templateHTMLSelectPut: SelectNullUndefined = templatePutForm?.querySelector('[name="html_id"]');

  const htmlSelectDelete: SelectNullUndefined = htmlDeleteForm?.querySelector('[name="id"]');

  const htmls = await getHTMLs().then(response => response);
  const htmlOptions = createOptionArray(htmls, false);

  templateHTMLSelectPost?.append(...htmlOptions);
  templateHTMLSelectPut?.append(...htmlOptions.map(element => element.cloneNode(true)));

  htmlSelectPut?.append(...htmlOptions.map(element => element.cloneNode(true)));
  htmlSelectDelete?.append(...htmlOptions.map(element => element.cloneNode(true)));
})();

// Download styles
(async function(): Promise<void> {
  const templateStylesSelectPost: SelectNullUndefined = templatePostForm?.querySelector('[name="style_ids"]');
  const templateStylesSelectPut: SelectNullUndefined = templatePutForm?.querySelector('[name="style_ids"]');

  const styleSelectDelete: SelectNullUndefined = styleDeleteForm?.querySelector('[name="id"]');

  const styles = await getStyles().then(response => response);
  const styleOptions = createOptionArray(styles, false);

  templateStylesSelectPost?.append(...styleOptions);
  templateStylesSelectPut?.append(...styleOptions.map(element => element.cloneNode(true)));

  styleSelectPut?.append(...styleOptions.map(element => element.cloneNode(true)));
  styleSelectDelete?.append(...styleOptions.map(element => element.cloneNode(true)));
})();

// Download formats
(async function(): Promise<void> {
  const templateFormatSelectPost: SelectNullUndefined = templatePostForm?.querySelector('[name="format"]');
  const templateFormatSelectPut: SelectNullUndefined = templatePutForm?.querySelector('[name="format"]');

  const formats = getFormats();
  const formatOptions = createOptionArray(formats, true);

  templateFormatSelectPost?.append(...formatOptions);
  templateFormatSelectPut?.append(...formatOptions.map(element => element.cloneNode(true)));
})();

// Download templates
(async function(): Promise<void> {
  const templateSelectDelete: SelectNullUndefined = templateDeleteForm?.querySelector('[name="id"]');

  const templates = await getTemplates().then(response => response);
  const templateOptions = createTemplateOptions(templates);

  templateSelectDelete?.append(...templateOptions);
  templateSelectPut?.append(...templateOptions.map(element => element.cloneNode(true)));
})();

async function getHTMLs(): Promise<HTML[]> {
  const response = await fetch('http://localhost:8000/internal/htmls/');
  const htmls = (await response.json() satisfies { htmls: HTML[] })['htmls'];

  return htmls;
}

async function getStyles(): Promise<Style[]> {
  const response = await fetch('http://localhost:8000/internal/styles/');
  const styles = (await response.json() satisfies { styles: Style[] })['styles'];

  return styles;
}

async function getTemplates(): Promise<Template[]> {
  const response = await fetch('http://localhost:8000/internal/templates/');
  const templates = (await response.json() satisfies { templates: Template[] })['templates'];

  return templates;
}

function getFormats(): Base[] {
  const formats: Base[] = [];

  for (const formatKey in AdFormat) {
    if (Number.isNaN(Number(formatKey))) {
      continue;
    }

    const formatName = splitIntoWordsByUpperCase(AdFormat[formatKey]);
    const format: Base = {
      id: String(formatKey),
      content: formatName,
    };

    formats.push(format);
  }

  return formats;
}

function createOptionArray(array: Base[], withContent: boolean): HTMLOptionElement[] {
  const optionsArray: HTMLOptionElement[] = [];

  for (const { id, content } of array) {
    if (withContent) {
      optionsArray.push(new Option(content, id));

      continue;
    }

    optionsArray.push(new Option(id, id));
  }

  return optionsArray;
}

function createTemplateOptions(array: Template[]): HTMLOptionElement[] {
  const optionsArray: HTMLOptionElement[] = [];

  for (const { id } of array) {
    optionsArray.push(new Option(id, id));
  }

  return optionsArray;
}

function splitIntoWordsByUpperCase(sentence: string): string {
  return sentence
    .split(/(?=[A-Z])/)
    .join(' ')
    .toLowerCase();
}

function createTemplateFromFormData(form: HTMLFormElement): TemplateInDB {
  const template: Template = {
    id: '',
    html_id: '',
    style_ids: new Array<string>(),
    format: 0,
    price: 0
  };
  const formData = new FormData(form);

  for (const [ key, value ] of formData.entries()) {
    switch (key) {
      case 'id':
      case 'html_id':
        template[key] = String(value);

        break;

      case 'format':
      case 'price':
        template[key] = Number(value);

        break;

      case 'style_ids':
        template[key].push(String(value));

        break;

      default:
        throw new Error('Fuck you');
    }
  }

  const templateToPut: TemplateInDB = {
    template,
  };

  return templateToPut;
}

async function deleteById(event: SubmitEvent): Promise<void> {
  event.preventDefault();

  const form = event.target as FormNull;

  if (!form) {
    return;
  }

  const id = String((new FormData(form)).get('id'));
  const path = form.action + id;

  await fetch(path, {
    method: 'DELETE',
  });

  form.reset();
  location.reload();
}

// Template (POST)
templatePostForm?.addEventListener('submit', async event => {
  event.preventDefault();

  const template: TemplateInDB = createTemplateFromFormData(templatePostForm);

  await fetch(templatePostForm.action, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json;charset=utf-8'
    },
    body: JSON.stringify(template),
  });

  templatePostForm.reset();
  location.reload();
});

// Template (PUT)
templateSelectPut?.addEventListener('change', async () => {
  const templatePath = 'http://localhost:8000/internal/templates/' + templateSelectPut.value;
  const inputFields = templatePutForm?.elements;

  if (inputFields === undefined) {
    return;
  }

  const response = await fetch(templatePath);
  const template = (await response.json() as { template: Template })['template'];

  for (const inputField of inputFields) {
    const field = (inputField as HTMLInputElement | HTMLSelectElement);

    if (field.disabled) {
      field.disabled = false;
    }

    const key = field.getAttribute('name') as string;

    switch (key) {
      case 'html_id':
        field.value = template[key];

        break;

      case 'format':
      case 'price':
        field.value = String(template[key]);

        break;

      case 'style_ids': {
        if (field instanceof HTMLSelectElement) {
          Array.from(field.options).forEach(option => {
            if (template[key].includes(option.value)) {
              option.selected = true;
            }
          });
        }

        break;
      }

      default:
        break;
    }
  }
});

templatePutForm?.addEventListener('submit', async event => {
  event.preventDefault();

  const template: TemplateInDB = createTemplateFromFormData(templatePutForm);

  await fetch(templatePutForm.action, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json;charset=utf-8'
    },
    body: JSON.stringify(template),
  });

  templatePutForm.reset();
  location.reload();
});

// Template (DELETE)
templateDeleteForm?.addEventListener('submit', deleteById);

// HTML (POST)
htmlPostForm?.addEventListener('submit', async event => {
  event.preventDefault();

  const html: HTML = {
    id: '',
    content: '',
  };
  const formData = new FormData(htmlPostForm);

  for (const [ key, value ] of formData.entries()) {
    switch (key) {
      case 'id':
      case 'content':
        html[key] = String(value);

        break;

      default:
        throw new Error('Parsing of HTML (POST) failed');
    }
  }

  const htmlToPut: HTMLInDB = {
    html,
  };

  await fetch(htmlPostForm.action, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json;charset=utf-8'
    },
    body: JSON.stringify(htmlToPut),
  });

  htmlPostForm.reset();
  location.reload();
});

// HTML (PUT)
htmlSelectPut?.addEventListener('change', async () => {
  const htmlPath = 'http://localhost:8000/internal/htmls/' + htmlSelectPut.value;
  const inputFields = htmlPutForm?.elements;

  if (inputFields === undefined) {
    return;
  }

  const response = await fetch(htmlPath);
  const html = (await response.json() satisfies { html: HTML })['html'];

  for (const inputField of inputFields) {
    const field = inputField as SelectInput;

    if (field.disabled) {
      field.disabled = false;
    }

    const key = field.getAttribute('name');

    if (!key) {
      return;
    }

    switch (key) {
      case 'content':
        field.value = html[key];

        break;

      default:
        break;
    }
  }
});

htmlPutForm?.addEventListener('submit', async event => {
  event.preventDefault();

  const html: HTML = {
    id: '',
    content: '',
  };
  const formData = new FormData(htmlPutForm);

  for (const [ key, value ] of formData.entries()) {
    switch (key) {
      case 'id':
      case 'content':
        html[key] = String(value);

        break;

      default:
        throw new Error('Parsing of HTML (PUT) failed');
    }
  }

  const htmlToPut: HTMLInDB = {
    html,
  };

  await fetch(htmlPutForm.action, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json;charset=utf-8'
    },
    body: JSON.stringify(htmlToPut),
  });

  htmlPutForm.reset();
  location.reload();
});

// HTML (DELETE)
htmlDeleteForm?.addEventListener('submit', deleteById);

// Style (POST)
stylePostForm?.addEventListener('submit', async event => {
  event.preventDefault();

  const style: Style = {
    id: '',
    content: '',
  };
  const formData = new FormData(stylePostForm);

  for (const [ key, value ] of formData.entries()) {
    switch (key) {
      case 'id':
      case 'content':
        style[key] = String(value);

        break;

      default:
        throw new Error('Parsing of HTML (POST) failed');
    }
  }

  const styleToPost: StyleInDB = {
    style,
  };

  await fetch(stylePostForm.action, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json;charset=utf-8'
    },
    body: JSON.stringify(styleToPost),
  });

  stylePostForm.reset();
  location.reload();
});

// Style (PUT)
styleSelectPut?.addEventListener('change', async () => {
  const stylePath = 'http://localhost:8000/internal/styles/' + styleSelectPut.value;
  const inputFields = stylePutForm?.elements;

  if (!inputFields) {
    return;
  }

  const response = await fetch(stylePath);
  const style = (await response.json() satisfies { style: Style })['style'];

  for (const inputField of inputFields) {
    const field = inputField as SelectInput;

    if (field.disabled) {
      field.disabled = false;
    }

    const key = field.getAttribute('name');

    if (!key) {
      return;
    }

    switch (key) {
      case 'content':
        field.value = style[key];

        break;

      default:
        break;
    }
  }
});

stylePutForm?.addEventListener('submit', async event => {
  event.preventDefault();

  const style: Style = {
    id: '',
    content: '',
  };
  const formData = new FormData(stylePutForm);

  for (const [ key, value ] of formData.entries()) {
    switch (key) {
      case 'id':
      case 'content':
        style[key] = String(value);

        break;

      default:
        throw new Error('Parsing of HTML (PUT) failed');
    }
  }

  const styleToPut: StyleInDB = {
    style,
  };

  await fetch(stylePutForm.action, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json;charset=utf-8'
    },
    body: JSON.stringify(styleToPut),
  });

  stylePutForm.reset();
  location.reload();
});

// Style (DELETE)
styleDeleteForm?.addEventListener('submit', deleteById);
