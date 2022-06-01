import { createStore, withProps } from '@ngneat/elf';
import {
  addEntities,
  deleteEntities,
  selectActiveEntity,
  selectAllEntities,
  setActiveId,
  updateEntities,
  withActiveId,
  withEntities,
  withUIEntities,
} from '@ngneat/elf-entities';
import { Observable } from 'rxjs';
import { withRequestsStatus } from '@ngneat/elf-requests';
import { Injectable } from '@angular/core';
import { withPagination } from '@ngneat/elf-pagination';
import { HttpClient } from '@angular/common/http';

export interface BlogPostUI {
  id: number;
}

export interface BlogPost {
  id: number;
}

// eslint-disable-next-line @typescript-eslint/no-empty-interface
export interface BlogPostProps {
}

@Injectable({ providedIn: 'root' })
export class BlogPostRepository {
  activeBlogPost$: Observable<BlogPost | undefined>;
  blogPost$: Observable<BlogPost[]>;

  private store;

  constructor(private http: HttpClient) {
    this.store = BlogPostRepository.createStore();
    this.blogPost$ = this.store.pipe(selectAllEntities());
    this.activeBlogPost$ = this.store.pipe(selectActiveEntity());
  }

  loadPosts() {
    const currentPage = this.store.value.pagination.currentPage;
    this.http.get('/api/posts', { params: { page: 1, perPage: 20 } });
  }

  addBlogPost(blogPost: BlogPost) {
    this.store.update(addEntities(blogPost));
  }

  updateBlogPost(id: BlogPost['id'], blogPost: Partial<BlogPost>) {
    this.store.update(updateEntities(id, blogPost));
  }

  deleteBlogPost(id: BlogPost['id']) {
    this.store.update(deleteEntities(id));
  }

  setActiveId(id: BlogPost['id']) {
    this.store.update(setActiveId(id));
  }

  private static createStore() {
    return createStore({ name: 'blogPost' },
      withProps<BlogPostProps>({}),
      withEntities<BlogPost>(),
      withUIEntities<BlogPostUI>(),
      withPagination(),
      withActiveId(),
      withRequestsStatus<'blog-post'>());
  }
}
