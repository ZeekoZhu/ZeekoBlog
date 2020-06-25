import { IArticlePostDto } from './Article';
import { appHeader } from './Utils';
import { PageModule } from './PageModule';

let editModule = () => {
    let saveBtn = $('#save');
    let titleInput = $('#title');
    let contentInput = $('#content');
    let docType = $('#docType');
    let id = $('#id').data('id');
    let tip = $('#tip');
    let summaryInput = $('#summary');
    const showTip = (str: string) => {
        tip.text(str);
        setTimeout(() => {
            tip.text('');
        }, 3000);
    };
    saveBtn.click(() => {
        let newArticle: IArticlePostDto = {
            content: contentInput.val(),
            title: titleInput.val(),
            summary: summaryInput.val(),
            docType: +docType.val(),
        };
        console.log(newArticle);

        if (id > 0) {
            fetch(`/api/Articles/${id}`,
                {
                    method: 'PUT',
                    body: JSON.stringify(newArticle),
                    headers: appHeader,
                })
                .then(resp => {
                    if (resp.status < 200 || resp.status >= 300) {
                        throw 'Http Error';
                    }
                    showTip('保存成功');
                })
                .catch(err => {
                    console.log(err);
                    showTip('保存失败');
                });
        } else {
            fetch('/api/Articles',
                {
                    method: 'POST',
                    body: JSON.stringify(newArticle),
                    headers: appHeader,
                })
                .then(resp => {
                    if (resp.status < 200 || resp.status >= 300) {
                        throw 'Http Error';
                    }
                    showTip('保存成功');
                })
                .catch(err => {
                    console.log(err);
                    showTip('保存失败');
                });
        }
    });
};

let listModule = () => {
    $('.deleteAct').click((e) => {
        e.preventDefault();
        let a = $(e.target);
        let id = a.data('id');
        fetch(`/api/Articles/${id}`,
            {
                method: 'DELETE',
                headers: appHeader,
            })
            .then(resp => {
                if (resp.status < 200 || resp.status >= 300) {
                    throw 'Http Error';
                } else {
                    window.location.reload();
                }
            });
    });
    $('.renderAct').click(async e => {
        e.preventDefault();
        let a = $(e.target);
        let id = a.data('id');
        fetch(`/api/Articles/render/${id}`,
            {
                method: 'put',
                headers: appHeader,
            })
            .then(resp => {
                if (resp.status < 200 || resp.status >= 300) {
                    throw 'Http Error';
                } else {
                    alert('操作成功');
                }
            });

    });
};

let loginModule = () => {
    let userNameInput = $('#userName');
    let pwdInput = $('#password');
    $('#loginBtn').click(async () => {
        let data = {
            userName: userNameInput.val(),
            password: pwdInput.val(),
        };
        const resp = await fetch('/api/token',
            {
                method: 'POST',
                headers: appHeader,
                body: JSON.stringify(data),
            });
        if (resp.status === 200) {
            window.location.reload();
        } else {
            const text = await resp.text();
            const errors = JSON.parse(text);
            alert(errors.UserName[0]);
        }
    });
};

PageModule.register('login', loginModule);
PageModule.register('edit', editModule);
PageModule.register('list', listModule);

$(document).ready(() => {
    appHeader.append('authorization', `Bearer ${localStorage.getItem('tk')}`);
    let pageModule = (window as any).__pageModule;
    if (pageModule) {
        PageModule.active(pageModule);
    }
});
