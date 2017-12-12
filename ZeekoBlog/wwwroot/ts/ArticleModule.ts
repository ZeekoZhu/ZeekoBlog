import { PageModule } from './PageModule';

let articleModule = () => {
    console.log('article actived');
    let headingSelector = [1, 2, 3, 4].map(n => `.content h${n}`).join(',');
    let headings = $(headingSelector).map((_, h: HTMLHeadingElement) => {
        let a = $('<a/>', {
            href: `#${h.id}`,
            text: h.innerText
        });
        return $('<li />', { class: `toc-${h.tagName}` }).append(a);
    });
    let tocList = $('.toc-list');
    tocList.append(headings);
};

PageModule.register('article-ro', articleModule);

$(document).ready(() => {
    let pageModule = (window as any).__pageModule;
    if (pageModule) {
        PageModule.active(pageModule);
    }
});
