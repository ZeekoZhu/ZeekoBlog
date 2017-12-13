import { PageModule } from './PageModule';
import { MobileSidebar as SidebarModule, SideBarEventType } from './MobileSidebar';

let articleModule = () => {
    console.log('article actived');

    // 生成 TOC
    let headingSelector = [2, 3, 4, 5].map(n => `.content h${n}`).join(',');
    let headings = $(headingSelector).map((_, h: HTMLHeadingElement) => {
        let a = $('<a/>', {
            href: `#${h.id}`,
            text: h.innerText
        });
        return $('<li />', { class: `toc-${h.tagName}` }).append(a);
    });
    let tocList = $('.toc-list');
    tocList.append(headings);


    // 为 action button 绑定事件
    let tocLink = $('.toc-list a');
    let sidebar = new SidebarModule();
    let tocLinkNavigate = e => {
        sidebar.hide();
        return true;
    }
    sidebar.bind(SideBarEventType.Hide, () => {
        tocLink.unbind('click', tocLinkNavigate);
        return true;
    });
    sidebar.bind(SideBarEventType.Show, () => {
        tocLink.bind('click', tocLinkNavigate);
        return true;
    });

};

let articleListModule = () => {
    console.log('article-list actived');
    let sidebar = new SidebarModule();
}

PageModule.register('article-ro', articleModule);
PageModule.register('article-list', articleListModule);

$(document).ready(() => {
    let pageModule = (window as any).__pageModule;
    if (pageModule) {
        PageModule.active(pageModule);
    }
});
