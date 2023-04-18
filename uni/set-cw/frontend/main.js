"use strict";
var AdFormat;
(function (AdFormat) {
    AdFormat[AdFormat["MagazineText"] = 0] = "MagazineText";
    AdFormat[AdFormat["MagazineTextWithMedia"] = 1] = "MagazineTextWithMedia";
    AdFormat[AdFormat["MagazinePhoto"] = 2] = "MagazinePhoto";
    AdFormat[AdFormat["Billboard"] = 3] = "Billboard";
    AdFormat[AdFormat["WebVideo"] = 4] = "WebVideo";
    AdFormat[AdFormat["WebImage"] = 5] = "WebImage";
})(AdFormat || (AdFormat = {}));
// Template (POST)
const templatePostForm = document.querySelector('[name="templatePost"]');
// Template (PUT)
const templatePutForm = document.querySelector('[name="templatePut"]');
const templateSelectPut = templatePutForm?.querySelector('[name="id"]');
// Template (DELETE)
const templateDeleteForm = document.querySelector('[name="templateDelete"]');
// HTML (POST)
const htmlPostForm = document.querySelector('[name="htmlPost"]');
// HTML (PUT)
const htmlPutForm = document.querySelector('[name="htmlPut"]');
const htmlSelectPut = htmlPutForm?.querySelector('[name="id"]');
// HTML (DELETE)
const htmlDeleteForm = document.querySelector('[name="htmlDelete"]');
// Style (POST)
const stylePostForm = document.querySelector('[name="stylePost"]');
// Style (PUT)
const stylePutForm = document.querySelector('[name="stylePut"]');
const styleSelectPut = stylePutForm?.querySelector('[name="id"]');
// Style (DELETE)
const styleDeleteForm = document.querySelector('[name="styleDelete"]');
// Download htmls
(async function () {
    const templateHTMLSelectPost = templatePostForm?.querySelector('[name="html_id"]');
    const templateHTMLSelectPut = templatePutForm?.querySelector('[name="html_id"]');
    const htmlSelectDelete = htmlDeleteForm?.querySelector('[name="id"]');
    const htmls = await getHTMLs().then(response => response);
    const htmlOptions = createOptionArray(htmls, false);
    templateHTMLSelectPost?.append(...htmlOptions);
    templateHTMLSelectPut?.append(...htmlOptions.map(element => element.cloneNode(true)));
    htmlSelectPut?.append(...htmlOptions.map(element => element.cloneNode(true)));
    htmlSelectDelete?.append(...htmlOptions.map(element => element.cloneNode(true)));
})();
// Download styles
(async function () {
    const templateStylesSelectPost = templatePostForm?.querySelector('[name="style_ids"]');
    const templateStylesSelectPut = templatePutForm?.querySelector('[name="style_ids"]');
    const styleSelectDelete = styleDeleteForm?.querySelector('[name="id"]');
    const styles = await getStyles().then(response => response);
    const styleOptions = createOptionArray(styles, false);
    templateStylesSelectPost?.append(...styleOptions);
    templateStylesSelectPut?.append(...styleOptions.map(element => element.cloneNode(true)));
    styleSelectPut?.append(...styleOptions.map(element => element.cloneNode(true)));
    styleSelectDelete?.append(...styleOptions.map(element => element.cloneNode(true)));
})();
// Download formats
(async function () {
    const templateFormatSelectPost = templatePostForm?.querySelector('[name="format"]');
    const templateFormatSelectPut = templatePutForm?.querySelector('[name="format"]');
    const formats = getFormats();
    const formatOptions = createOptionArray(formats, true);
    templateFormatSelectPost?.append(...formatOptions);
    templateFormatSelectPut?.append(...formatOptions.map(element => element.cloneNode(true)));
})();
// Download templates
(async function () {
    const templateSelectDelete = templateDeleteForm?.querySelector('[name="id"]');
    const templates = await getTemplates().then(response => response);
    const templateOptions = createTemplateOptions(templates);
    templateSelectDelete?.append(...templateOptions);
    templateSelectPut?.append(...templateOptions.map(element => element.cloneNode(true)));
})();
async function getHTMLs() {
    const response = await fetch('http://localhost:8000/internal/htmls/');
    const htmls = (await response.json())['htmls'];
    return htmls;
}
async function getStyles() {
    const response = await fetch('http://localhost:8000/internal/styles/');
    const styles = (await response.json())['styles'];
    return styles;
}
async function getTemplates() {
    const response = await fetch('http://localhost:8000/internal/templates/');
    const templates = (await response.json())['templates'];
    return templates;
}
function getFormats() {
    const formats = [];
    for (const formatKey in AdFormat) {
        if (Number.isNaN(Number(formatKey))) {
            continue;
        }
        const formatName = splitIntoWordsByUpperCase(AdFormat[formatKey]);
        const format = {
            id: String(formatKey),
            content: formatName,
        };
        formats.push(format);
    }
    return formats;
}
function createOptionArray(array, withContent) {
    const optionsArray = [];
    for (const { id, content } of array) {
        if (withContent) {
            optionsArray.push(new Option(content, id));
            continue;
        }
        optionsArray.push(new Option(id, id));
    }
    return optionsArray;
}
function createTemplateOptions(array) {
    const optionsArray = [];
    for (const { id } of array) {
        optionsArray.push(new Option(id, id));
    }
    return optionsArray;
}
function splitIntoWordsByUpperCase(sentence) {
    return sentence
        .split(/(?=[A-Z])/)
        .join(' ')
        .toLowerCase();
}
function createTemplateFromFormData(form) {
    const template = {
        id: '',
        html_id: '',
        style_ids: new Array(),
        format: 0,
        price: 0
    };
    const formData = new FormData(form);
    for (const [key, value] of formData.entries()) {
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
    const templateToPut = {
        template,
    };
    return templateToPut;
}
async function deleteById(event) {
    event.preventDefault();
    const form = event.target;
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
templatePostForm?.addEventListener('submit', async (event) => {
    event.preventDefault();
    const template = createTemplateFromFormData(templatePostForm);
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
    const template = (await response.json())['template'];
    for (const inputField of inputFields) {
        const field = inputField;
        if (field.disabled) {
            field.disabled = false;
        }
        const key = field.getAttribute('name');
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
templatePutForm?.addEventListener('submit', async (event) => {
    event.preventDefault();
    const template = createTemplateFromFormData(templatePutForm);
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
htmlPostForm?.addEventListener('submit', async (event) => {
    event.preventDefault();
    const html = {
        id: '',
        content: '',
    };
    const formData = new FormData(htmlPostForm);
    for (const [key, value] of formData.entries()) {
        switch (key) {
            case 'id':
            case 'content':
                html[key] = String(value);
                break;
            default:
                throw new Error('Parsing of HTML (POST) failed');
        }
    }
    const htmlToPut = {
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
    const html = (await response.json())['html'];
    for (const inputField of inputFields) {
        const field = inputField;
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
htmlPutForm?.addEventListener('submit', async (event) => {
    event.preventDefault();
    const html = {
        id: '',
        content: '',
    };
    const formData = new FormData(htmlPutForm);
    for (const [key, value] of formData.entries()) {
        switch (key) {
            case 'id':
            case 'content':
                html[key] = String(value);
                break;
            default:
                throw new Error('Parsing of HTML (PUT) failed');
        }
    }
    const htmlToPut = {
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
stylePostForm?.addEventListener('submit', async (event) => {
    event.preventDefault();
    const style = {
        id: '',
        content: '',
    };
    const formData = new FormData(stylePostForm);
    for (const [key, value] of formData.entries()) {
        switch (key) {
            case 'id':
            case 'content':
                style[key] = String(value);
                break;
            default:
                throw new Error('Parsing of HTML (POST) failed');
        }
    }
    const styleToPost = {
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
    const style = (await response.json())['style'];
    for (const inputField of inputFields) {
        const field = inputField;
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
stylePutForm?.addEventListener('submit', async (event) => {
    event.preventDefault();
    const style = {
        id: '',
        content: '',
    };
    const formData = new FormData(stylePutForm);
    for (const [key, value] of formData.entries()) {
        switch (key) {
            case 'id':
            case 'content':
                style[key] = String(value);
                break;
            default:
                throw new Error('Parsing of HTML (PUT) failed');
        }
    }
    const styleToPut = {
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
