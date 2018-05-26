/**
 * 用来控制侧边栏，需要在 DOM 加载完成后实例化
 */

export enum SideBarEventType {
    Show, Hide
}
export class MobileSidebar {
    actionBtn: ZeptoCollection;
    sidebar: ZeptoCollection;
    isShowing: boolean = false;
    onShowDelegates: Function[] = [];
    onHideDelegates: Function[] = [];
    constructor() {
        this.actionBtn = $('.action-btn');
        this.sidebar = $('.side-container');
        this.actionBtn.bind('tap', (e) => {
            this.isShowing ? this.hide() : this.show();
            return true;
        });
    }

    bind(type: SideBarEventType, func: () => boolean) {
        switch (type) {
            case SideBarEventType.Show:
                this.onShowDelegates.push(func);
                break;
            case SideBarEventType.Hide:
                this.onHideDelegates.push(func);
                break;
            default:
                throw 'Invalid event type';
        }
    }

    unbind(type: SideBarEventType, func: () => boolean) {
        let delegates: Function[] = [];
        switch (type) {
            case SideBarEventType.Show:
                delegates = this.onShowDelegates;
                break;
            case SideBarEventType.Hide:
                delegates = this.onHideDelegates;
                break;
        }
        let index = delegates.indexOf(func);
        if (index >= 0) {
            delegates.splice(index, 1);
        }
    }
    
    show() {
        if (this.onShowDelegates.length > 0 && (this.onShowDelegates.some(fn => fn()) === false)) {
            return;
        }
        this.sidebar.animate({ opacity: 1, height: '100%' }, 200, 'ease-in');
        $('body').css('overflow', 'hidden');
        this.isShowing = true;
    }

    hide() {
        if (this.onHideDelegates.length > 0 && (this.onHideDelegates.some(fn => fn()) === false)) {
            return;
        }
        this.sidebar.animate({ opacity: 0, height: 0 }, 200, 'ease-out');
        $('body').css('overflow', 'auto');
        this.isShowing = false;
    }
}