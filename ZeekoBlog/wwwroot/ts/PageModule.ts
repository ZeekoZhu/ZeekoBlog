export class PageModule {
    static modules: { name: string; func: Function }[] = [];
    static register(name: string, func: Function) {
        PageModule.modules.push({ name: name, func: func });
    }
    static active(name: string) {
        let module = PageModule.modules.find(m => m.name === name);
        if (module) {
            module.func();
        }
    }
}