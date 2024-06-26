package ru.hits.thirdcourseservice.dto;

import lombok.*;

@Data
@AllArgsConstructor
@NoArgsConstructor
@Getter
@Setter
public class PageInfoDto {

    private Integer total;

    private Integer pageNumber;

    private Integer pageSize;

}