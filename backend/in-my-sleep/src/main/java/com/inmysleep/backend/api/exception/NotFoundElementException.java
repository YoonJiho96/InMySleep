package com.inmysleep.backend.api.exception;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.ResponseStatus;

@ResponseStatus(HttpStatus.NOT_FOUND)
public class NotFoundElementException extends RuntimeException {
    public NotFoundElementException(String message) {
        super(message);
    }
}
