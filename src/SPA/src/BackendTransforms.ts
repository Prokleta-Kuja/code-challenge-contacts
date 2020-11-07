export class BackendTransforms {
  protected transformOptions(options: RequestInit) {
    options.cache = "no-store";
    return Promise.resolve(options);
  }

  protected transformResult(
    url: string,
    response: Response,
    processor: (response: Response) => any
  ) {
    // Handle any processing
    return processor(response);
  }
}
