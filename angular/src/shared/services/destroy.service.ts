import { OnDestroy } from '@angular/core';
import { pipe, Subject, takeUntil } from 'rxjs';

export class DestroyService implements OnDestroy {
  readonly destroy$ = new Subject<void>();
  get pipe() {
    return pipe(takeUntil(this.destroy$));
  }
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
