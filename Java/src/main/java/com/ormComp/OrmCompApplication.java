package com.ormComp;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.autoconfigure.jdbc.DataSourceAutoConfiguration;


@SpringBootApplication
public class OrmCompApplication {

	public static void main(String[] args) {
		SpringApplication.run(OrmCompApplication.class, args);
	}

}
