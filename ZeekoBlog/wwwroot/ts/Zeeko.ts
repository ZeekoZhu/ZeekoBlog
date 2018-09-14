import {Article, IArticlePostDto} from './Article';
import { appHeader } from './Utils';
import {PageModule} from './PageModule';

let editModule = () => {
    let saveBtn = $('#save');
    let titleInput = $('#title');
    let contentInput = $('#content');
    let docType = $('#docType');
    let id = $('#id').val();
    let successTips = $('#save-success');
    let errorTips = $('#save-error');
    let summaryInput = $('#summary');
    saveBtn.click(() => {
        let newArticle:IArticlePostDto = {
            content: contentInput.val(),
            title: titleInput.val(),
            summary: summaryInput.val(),
            docType: +docType.val()
        };
        console.log(newArticle);

        if (id) {
            fetch(`/api/Articles/${id}`,
                {
                    method: 'PUT',
                    body: JSON.stringify(newArticle),
                    headers: appHeader
                })
                .then(resp => {
                    if (resp.status < 200 || resp.status >= 300) {
                        throw 'Http Error';
                    }
                    successTips.show();
                    setTimeout(() => successTips.hide(), 3000);
                })
                .catch(err => {
                    console.log(err);
                    errorTips.show();
                    setTimeout(() => errorTips.hide(), 3000);
                });
        } else {
            fetch('/api/Articles',
                {
                    method: 'POST',
                    body: JSON.stringify(newArticle),
                    headers: appHeader
                })
                .then(resp => {
                    if (resp.status < 200 || resp.status >= 300) {
                        throw 'Http Error';
                    }
                    successTips.show();
                    setTimeout(() => successTips.hide(), 3000);
                })
                .catch(err => {
                    console.log(err);
                    errorTips.show();
                    setTimeout(() => errorTips.hide(), 3000);
                });
        }
    });
}

let listModule = () => {
    $('.delete-art').click((e) => {
        e.preventDefault();
        let a = $(e.target);
        let id = a.attr('art-id');
        fetch(`/api/Articles/${id}`,
            {
                method: 'DELETE',
                headers: appHeader
            })
            .then(resp => {
                if (resp.status < 200 || resp.status >= 300) {
                    throw 'Http Error';
                }
                $(`#a-${id}`).remove();
            });
    });
}

let loginModule = () => {
    let userNameInput = $('#userName');
    let pwdInput = $('#password');
    $('#loginBtn').click(() => {
        let data = {
            userName: userNameInput.val(),
            password: pwdInput.val()
        }
        fetch('/api/token',
            {
                method: 'POST',
                headers: appHeader,
                body: JSON.stringify(data)
            })
            .then(resp => {
                if (resp.status === 200) {
                    let token = resp.headers.get('tk').substring(7);
                    localStorage.setItem('tk', token);
                    window.open('/api/Token/ToPage/?tk=' + token, '_self');
                }
            });
    });
}

PageModule.register('login',loginModule);
PageModule.register('edit',editModule);
PageModule.register('list', listModule);


$(document).ready(() => {
    appHeader.append("authorization", `Bearer ${localStorage.getItem('tk')}`);
    let pageModule = (window as any).__pageModule;
    if (pageModule) {
        PageModule.active(pageModule);
    }
});